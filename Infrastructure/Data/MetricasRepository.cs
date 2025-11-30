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
            // =================================================================================
            // PASO 1: Obtener el Semestre Actual (Referencia para saber hasta dónde buscar)
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
            // PASO 2: Construir la consulta base (INSCRIPCIONES + ESTUDIANTE) del periodo ACTUAL
            // =================================================================================
            // Esto sirve para los KPIs del momento (Cant estudiantes, reprobación actual, etc.)
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
            
            // Reprobados (Promedio < 60 O Estado == 'REP')
            int totalReprobados = datosAcademicos.Count(x => (x.Promedio != null && x.Promedio < 60) || x.CodEstadoCurso == "REP");
            
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
            // PASO 4: Top Materias Reprobadas (Del periodo actual)
            // =================================================================================
            var topMateriasIds = await queryBase
                .GroupBy(x => x.ins.CodMateria)
                .Select(g => new 
                {
                    CodMateria = g.Key.GetValueOrDefault(), 
                    Reprobados = g.Count(y => (y.ins.Promedio != null && y.ins.Promedio < 60) || y.ins.CodEstadoCurso == "REP")
                })
                .OrderByDescending(x => x.Reprobados)
                .Take(5)
                .ToListAsync();

            var idsMat = topMateriasIds.Select(m => m.CodMateria).ToList();
            
            var nombresMaterias = await _context.Materias
                .AsNoTracking()
                .Where(m => idsMat.Contains(m.CodMateria))
                .ToDictionaryAsync(m => m.CodMateria, m => m.NombreMateria); 

            var listaMateriasDto = topMateriasIds.Select(x => new MateriaReprobacionDto
            {
                NombreMateria = nombresMaterias.ContainsKey(x.CodMateria) ? nombresMaterias[x.CodMateria] : "Desconocido",
                Reprobados = x.Reprobados
            }).ToList();


            // =================================================================================
            // PASO 5: Evolución Histórica (Lógica de Inscripciones por Materia)
            // =================================================================================
            // Aquí cambiamos la lógica: Contamos TODAS las inscripciones históricas agrupadas por semestre.
            
            // 1. Armamos la Query Histórica (INSCRIPCIONES + ESTUDIANTES + SEMESTRE)
            var queryHistorica = from ins in _context.Inscripciones.AsNoTracking()
                                 join est in _context.Estudiantes.AsNoTracking() 
                                     on ins.CodEstudiante equals est.CodEstudiante
                                 join sem in _context.Semestres.AsNoTracking() 
                                     on ins.CodSemestre equals sem.CodSemestre
                                 // Filtramos fechas: Solo semestres anteriores o iguales al actual
                                 where sem.FechaInicio <= semestreActual.FechaInicio
                                 select new { ins, est, sem };

            // 2. Si pidieron una carrera, filtramos TODO el historial por esa carrera
            if (codCarrera.HasValue)
            {
                queryHistorica = queryHistorica.Where(x => x.est.CodCarrera == codCarrera.Value);
            }

            // 3. Agrupamos por Semestre y Contamos
            var datosEvolucion = await queryHistorica
                .GroupBy(x => new { x.sem.Nombre, x.sem.Anio, x.sem.Termino, x.sem.FechaInicio })
                .Select(g => new 
                {
                    NombreSemestre = g.Key.Nombre ?? (g.Key.Anio + " - " + g.Key.Termino),
                    FechaInicio = g.Key.FechaInicio,
                    // Cuenta cuántas materias se inscribieron en total en ese semestre (Volumen de actividad)
                    TotalInscripciones = g.Count() 
                })
                .OrderByDescending(x => x.FechaInicio) // Ordenamos descendente para tomar los últimos
                .Take(6) // Tomamos los últimos 6
                .ToListAsync();

            // 4. Reordenamos ascendente (Cronológico) para que la gráfica se dibuje bien (Izq -> Der)
            datosEvolucion = datosEvolucion.OrderBy(x => x.FechaInicio).ToList();

            var evolucionDto = new EvolucionIngresosDto
            {
                Periodos = datosEvolucion.Select(x => x.NombreSemestre).ToList(),
                CantidadIngresos = datosEvolucion.Select(x => x.TotalInscripciones).ToList()
            };


            // =================================================================================
            // PASO 6: Rendimiento por Carrera (Solo si no se filtró una carrera específica)
            // =================================================================================
            List<RendimientoCarreraDto>? rendimientoCarrera = null;

            if (!codCarrera.HasValue)
            {
                var statsCarreras = await queryBase
                    .GroupBy(x => x.est.CodCarrera)
                    .Select(g => new
                    {
                        CodCarrera = g.Key.GetValueOrDefault(),
                        Total = g.Count(),
                        Reprobados = g.Count(y => (y.ins.Promedio != null && y.ins.Promedio < 60) || y.ins.CodEstadoCurso == "REP")
                    })
                    .ToListAsync();

                var idsCarreras = statsCarreras.Select(c => c.CodCarrera).ToList();
                
                var nombresCarreras = await _context.Carreras
                    .AsNoTracking()
                    .Where(c => idsCarreras.Contains(c.CodCarrera))
                    .ToDictionaryAsync(c => c.CodCarrera, c => c.NombreCarrera); 

                rendimientoCarrera = statsCarreras.Select(s => new RendimientoCarreraDto
                {
                    NombreCarrera = nombresCarreras.ContainsKey(s.CodCarrera) ? nombresCarreras[s.CodCarrera] : $"Carrera {s.CodCarrera}",
                    Reprobados = s.Reprobados,
                    Aprobados = s.Total - s.Reprobados
                }).ToList();
            }

            // =================================================================================
            // PASO 7: Retorno Final
            // =================================================================================
            return new DashboardDto
            {
                CantEstudiantes = cantEstudiantes,
                TasaReprobacion = tasaReprobacion,
                TasaGraduados = 0, 
                PromedioCarrera = promedioCarrera,
                MateriasMayorReprobacion = listaMateriasDto,
                EvolucionIngresos = evolucionDto,
                RendimientoCarrera = rendimientoCarrera
            };
        }
    }
}