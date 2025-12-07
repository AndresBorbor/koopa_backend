using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;
using KoopaBackend.Domain.Entities; // Asegúrate de tener este using para la entidad Semestre

namespace KoopaBackend.Infrastructure.Repositories
{
    public class MetricasRepository : IMetricasRepository
    {
        private readonly KoopaDbContext _context;

        public MetricasRepository(KoopaDbContext context)
        {
            _context = context;
        }

        // 1. Actualizamos la firma para aceptar nulos en anio y termino
        public async Task<DashboardDto> ObtenerMetricasAsync(int? codCarrera, int? anio, string? termino)
{
    // CONFIGURACIÓN: Nota mínima para aprobar (Escala de 10)
    decimal notaMinima = 6.0m;

    // =================================================================================
    // PASO 1: Determinar el Contexto de Tiempo (Semestre)
    // =================================================================================
    Semestre? semestreActual = null;
    int? codSemestreActual = null;

    if (anio.HasValue && !string.IsNullOrEmpty(termino))
    {
        semestreActual = await _context.Semestres
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Anio == anio && s.Termino == termino);

        if (semestreActual == null)
        {
            throw new Exception($"No se encontró el periodo {anio} - {termino}");
        }
        codSemestreActual = semestreActual.CodSemestre;
    }

    // =================================================================================
    // PASO 2: Construir la Consulta Base
    // =================================================================================
    var queryBase = from ins in _context.Inscripciones.AsNoTracking()
                    join est in _context.Estudiantes.AsNoTracking()
                        on ins.CodEstudiante equals est.CodEstudiante
                    select new { ins, est };

    // Filtros
    if (codSemestreActual.HasValue)
    {
        queryBase = queryBase.Where(x => x.ins.CodSemestre == codSemestreActual.Value);
    }

    if (codCarrera.HasValue)
    {
        queryBase = queryBase.Where(x => x.est.CodCarrera == codCarrera.Value);
    }

    // =================================================================================
    // PASO 3: Métricas Escalares (CORREGIDO)
    // =================================================================================
    
    // A. Cantidad de Estudiantes (Personas únicas)
    int cantEstudiantes = await queryBase
        .Select(x => x.est.CodEstudiante)
        .Distinct()
        .CountAsync();

    // B. Reprobados (CORREGIDO: Lógica igual a Graduados)
    // Buscamos los estudiantes únicos que hayan reprobado al menos una materia
    var queryReprobados = queryBase.Where(x => 
        (x.ins.Promedio != null && x.ins.Promedio < notaMinima) || 
        x.ins.CodEstadoCurso == "REP"
    ).Select(x => x.ins.CodEstudiante);

    // Usamos Distinct() para contar personas, no exámenes fallidos
    int totalReprobados = await queryReprobados.Distinct().CountAsync();

    double tasaReprobacion = cantEstudiantes > 0
        ? Math.Round(((double)totalReprobados / cantEstudiantes) * 100, 2)
        : 0;

    // Calculamos el promedio directo en base de datos (sin traer lista a memoria)
    double promedioCarrera = 0;
    var promediosValidos = queryBase.Where(x => x.ins.Promedio != null);
    
    // Verificamos si hay notas antes de promediar para evitar excepción de división por cero
    if (await promediosValidos.AnyAsync())
    {
        decimal prom = await promediosValidos.AverageAsync(x => x.ins.Promedio!.Value);
        promedioCarrera = Math.Round((double)prom, 2);
    }

    // =================================================================================
    // C. Tasa de Graduados
    // =================================================================================
    var queryGraduados = from q in queryBase
                            join mat in _context.Materias.AsNoTracking()
                                on q.ins.CodMateria equals mat.CodMateria
                            where mat.CodTipoCredito == 6 // Tesis/Graduación
                            && q.ins.Promedio != null 
                            && q.ins.Promedio >= notaMinima 
                            select q.ins.CodEstudiante;

    int totalGraduados = await queryGraduados.Distinct().CountAsync();

    double tasaGraduados = cantEstudiantes > 0
        ? Math.Round(((double)totalGraduados / cantEstudiantes) * 100, 2)
        : 0;

    // =================================================================================
    // PASO 4: Top Materias Reprobadas
    // =================================================================================
    var topMateriasReprobadas = await queryBase
        .GroupBy(x => x.ins.CodMateria)
        .Select(g => new
        {
            CodMateria = g.Key.GetValueOrDefault(),
            Reprobados = g.Count(y => (y.ins.Promedio != null && y.ins.Promedio < notaMinima) || y.ins.CodEstadoCurso == "REP")
        })
        .OrderByDescending(x => x.Reprobados)
        .Take(5)
        .ToListAsync();

    var idsMatRep = topMateriasReprobadas.Select(m => m.CodMateria).ToList();

    // =================================================================================
    // PASO 4.5: Top Materias Más Pobladas
    // =================================================================================
    var topMateriasPobladas = await queryBase
        .GroupBy(x => x.ins.CodMateria)
        .Select(g => new
        {
            CodMateria = g.Key.GetValueOrDefault(),
            TotalInscritos = g.Count()
        })
        .OrderByDescending(x => x.TotalInscritos)
        .Take(5)
        .ToListAsync();

    var idsMatPob = topMateriasPobladas.Select(m => m.CodMateria).ToList();

    // Union y Nombres
    var todosIdsMaterias = idsMatRep.Union(idsMatPob).Distinct().ToList();

    var nombresMaterias = await _context.Materias
        .AsNoTracking()
        .Where(m => todosIdsMaterias.Contains(m.CodMateria))
        .ToDictionaryAsync(m => m.CodMateria, m => m.NombreMateria);

    var listaMateriasRepDto = topMateriasReprobadas.Select(x => new MateriaReprobacionDto
    {
        NombreMateria = nombresMaterias.ContainsKey(x.CodMateria) ? nombresMaterias[x.CodMateria] : "Desconocido",
        Reprobados = x.Reprobados
    }).ToList();

    var listaMateriasPobDto = topMateriasPobladas.Select(x => new MateriaPobladaDto
    {
        NombreMateria = nombresMaterias.ContainsKey(x.CodMateria) ? nombresMaterias[x.CodMateria] : "Desconocido",
        CantidadInscritos = x.TotalInscritos
    }).ToList();

    // =================================================================================
    // PASO 5: Evolución Histórica
    // =================================================================================
var queryHistorica = from ins in _context.Inscripciones.AsNoTracking()
                                 join est in _context.Estudiantes.AsNoTracking()
                                     on ins.CodEstudiante equals est.CodEstudiante
                                 join sem in _context.Semestres.AsNoTracking()
                                     on ins.CodSemestre equals sem.CodSemestre
                                 select new { ins, est, sem };

            // Filtros para la histórica
            if (codCarrera.HasValue)
            {
                queryHistorica = queryHistorica.Where(x => x.est.CodCarrera == codCarrera.Value);
            }

            if (semestreActual != null)
            {
                queryHistorica = queryHistorica.Where(x => x.sem.FechaInicio <= semestreActual.FechaInicio);
            }

            var datosEvolucion = await queryHistorica
                .GroupBy(x => new { x.sem.Nombre, x.sem.Anio, x.sem.Termino, x.sem.FechaInicio })
                .Select(g => new
                {
                    NombreSemestre = g.Key.Nombre ?? (g.Key.Anio + " - " + g.Key.Termino),
                    FechaInicio = g.Key.FechaInicio,
                    TotalInscripciones = g.Count()
                })
                .OrderBy(x => x.FechaInicio) // CORRECCIÓN: Orden ascendente directo (Old -> New)
                // .Take(6) <--- ESTO ERA LO QUE LIMITABA A 2024. ¡ELIMINADO!
                .ToListAsync();

            // Ya no hace falta el reordenamiento en memoria porque lo hicimos en la query
            // datosEvolucion = datosEvolucion.OrderBy(x => x.FechaInicio).ToList(); 

            var evolucionDto = new EvolucionIngresosDto
            {
                Periodos = datosEvolucion.Select(x => x.NombreSemestre).ToList(),
                CantidadIngresos = datosEvolucion.Select(x => x.TotalInscripciones).ToList()
            };

    // =================================================================================
    // PASO 6: Rendimiento por Carrera
    // =================================================================================
    var statsCarreras = await queryBase
        .GroupBy(x => x.est.CodCarrera)
        .Select(g => new
        {
            CodCarrera = g.Key.GetValueOrDefault(),
            Total = g.Count(), // Total de registros (materias inscritas)
            Reprobados = g.Count(y => (y.ins.Promedio != null && y.ins.Promedio < notaMinima) || y.ins.CodEstadoCurso == "REP")
        })
        .ToListAsync();

    var idsCarreras = statsCarreras.Select(c => c.CodCarrera).ToList();

    var nombresCarreras = await _context.Carreras
        .AsNoTracking()
        .Where(c => idsCarreras.Contains(c.CodCarrera))
        .ToDictionaryAsync(c => c.CodCarrera, c => c.NombreCarrera);

    var rendimientoCarrera = statsCarreras.Select(s => new RendimientoCarreraDto
    {
        NombreCarrera = nombresCarreras.ContainsKey(s.CodCarrera) ? nombresCarreras[s.CodCarrera] : $"Carrera {s.CodCarrera}",
        Reprobados = s.Reprobados,
        Aprobados = s.Total - s.Reprobados
    }).ToList();

    // =================================================================================
    // PASO 7: Retorno
    // =================================================================================
    return new DashboardDto
    {
        CantEstudiantes = cantEstudiantes,
        
        TotalReprobados = totalReprobados, // Ahora son estudiantes únicos
        TasaReprobacion = tasaReprobacion,
        
        TotalGraduados = totalGraduados,
        TasaGraduados = tasaGraduados,
        
        PromedioCarrera = promedioCarrera,
        MateriasMayorReprobacion = listaMateriasRepDto,
        MateriasMasPobladas = listaMateriasPobDto,
        EvolucionIngresos = evolucionDto,
        RendimientoCarrera = rendimientoCarrera
    };
}
    }
}