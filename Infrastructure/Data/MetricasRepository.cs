using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class MetricasRepository : IMetricasRepository
    {
        private readonly KoopaDbContext _context;

        public MetricasRepository(KoopaDbContext context)
        {
            _context = context;
        }

public async Task<DashboardDto> ObtenerMetricasAsync(int? codCarrera, int anio, string termino)
{
    // CORRECCIÓN AQUÍ: Usamos 'decimal' (con la 'm' al final) para que sea compatible con la BD
    decimal notaMinima = 6.0m; 

    // =================================================================================
    // PASO 1: Obtener el Semestre Actual
    // =================================================================================
    var semestreActual = await _context.Semestres
        .AsNoTracking()
        .FirstOrDefaultAsync(s => s.Anio == anio && s.Termino == termino);

    if (semestreActual == null)
    {
        throw new Exception($"No se encontró el periodo {anio} - {termino}");
    }

    int codSemestreActual = semestreActual.CodSemestre;

    // =================================================================================
    // PASO 2: Consulta Base
    // =================================================================================
    var queryBase = from ins in _context.Inscripciones.AsNoTracking()
                    join est in _context.Estudiantes.AsNoTracking()
                        on ins.CodEstudiante equals est.CodEstudiante
                    where ins.CodSemestre == codSemestreActual
                    select new { ins, est };

    if (codCarrera.HasValue)
    {
        queryBase = queryBase.Where(x => x.est.CodCarrera == codCarrera.Value);
    }

            // =================================================================================
            // PASO 3: Métricas Escalares (Del periodo actual)
            // =================================================================================
            
            // A. Cantidad de Estudiantes (Personas únicas)
            int cantEstudiantes = await queryBase
                .Select(x => x.est.CodEstudiante)
                .Distinct()
                .CountAsync();
  
            // B. Datos para tasas (En memoria para cálculo rápido)
            var datosAcademicos = await queryBase
                .Select(x => new { x.ins.Promedio, x.ins.CodEstadoCurso })
                .ToListAsync();
         

            int totalRegistros = datosAcademicos.Count;
            
            // Reprobados (Promedio < 6 O Estado == 'REP')
            int totalReprobados = datosAcademicos.Count(x => (x.Promedio != null && x.Promedio < 6) || x.CodEstadoCurso == "REP");


            double tasaReprobacion = totalRegistros > 0 
                ? Math.Round(((double)totalReprobados / totalRegistros) * 100, 2) 
                : 0;

    double promedioCarrera = 0;
    if (datosAcademicos.Any(x => x.Promedio != null))
    {
        decimal promedioDecimal = datosAcademicos
                                    .Where(x => x.Promedio != null)
                                    .Average(x => x.Promedio!.Value);
        promedioCarrera = Math.Round((double)promedioDecimal, 2);
    }

    // =================================================================================
    // PASO 4: Top Materias Reprobadas
    // =================================================================================
    var topMateriasReprobadas = await queryBase
        .GroupBy(x => x.ins.CodMateria)
        .Select(g => new
        {
            CodMateria = g.Key.GetValueOrDefault(),
            // Comparación corregida
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

    // Union de IDs para consultar nombres una sola vez
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
                         where sem.FechaInicio <= semestreActual.FechaInicio
                         select new { ins, est, sem };

    if (codCarrera.HasValue)
    {
        queryHistorica = queryHistorica.Where(x => x.est.CodCarrera == codCarrera.Value);
    }

    var datosEvolucion = await queryHistorica
        .GroupBy(x => new { x.sem.Nombre, x.sem.Anio, x.sem.Termino, x.sem.FechaInicio })
        .Select(g => new
        {
            NombreSemestre = g.Key.Nombre ?? (g.Key.Anio + " - " + g.Key.Termino),
            FechaInicio = g.Key.FechaInicio,
            TotalInscripciones = g.Count()
        })
        .OrderByDescending(x => x.FechaInicio)
        .Take(6)
        .ToListAsync();

    datosEvolucion = datosEvolucion.OrderBy(x => x.FechaInicio).ToList();

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
            Total = g.Count(),
            // Comparación corregida
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
        TasaReprobacion = tasaReprobacion,
        TasaGraduados = 0,
        PromedioCarrera = promedioCarrera,
        MateriasMayorReprobacion = listaMateriasRepDto,
        MateriasMasPobladas = listaMateriasPobDto,
        EvolucionIngresos = evolucionDto,
        RendimientoCarrera = rendimientoCarrera
    };
}
    }
}