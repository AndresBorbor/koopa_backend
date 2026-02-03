using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoopaBackend.Application.Services
{
    public class MetricasService
    {
        private readonly IMetricasRepository _repository;

        public MetricasService(IMetricasRepository repository)
        {
            _repository = repository;
        }

        public async Task<DashboardDto> ObtenerDashboardAsync(
            int? codCarrera,
            int? anio,
            string? termino)
        {

            // ============================
            // 1. Validaciones de negocio
            // ============================
            if (anio.HasValue && (anio < 1900 || anio > 3000))
                throw new ArgumentException("El año proporcionado no es válido.");

            // ============================
            // 2. Obtener datos base
            // ============================
            List<VW_MetricasCarreraPeriodo> metricasPeriodo = await _repository
                .ObtenerMetricasCarreraPeriodosAsync(codCarrera, anio, termino);
            List<VW_MetricasCarreraAnio> metricasAnio =metricasAnio = await _repository
                    .ObtenerMetricasCarreraAniosAsync(codCarrera, anio);

            var materias = await _repository
                .ObtenerMetricasMateriasAsync(codCarrera, anio, termino);

            var graduados = await _repository
                .ObtenerGraduadosAsync(codCarrera, anio);

            // ============================
            // 3. Totales generales
            // ============================
            int totalEstudiantes;
            int totalReprobados;
            int totalInscripciones;

            if (!string.IsNullOrEmpty(termino))
            {
                totalEstudiantes = metricasPeriodo.Sum(x => x.CantidadEstudiantes);
                totalReprobados = metricasPeriodo.Sum(x => x.CantidadReprobados);
                totalInscripciones = metricasPeriodo.Sum(x => x.CantidadInscripciones);
            }
            else
            {
                totalEstudiantes = metricasAnio.Sum(x => x.CantidadEstudiantes);
                totalReprobados = metricasAnio.Sum(x => x.CantidadReprobados);
                totalInscripciones = metricasAnio.Sum(x => x.CantidadInscripciones);
            }

            double tasaReprobacion = totalInscripciones == 0
                ? 0
                : Math.Round((double)totalReprobados * 100 / totalInscripciones, 2);

            int totalGraduados = graduados.Sum(x => x.CantidadGraduados);

            // ============================
            // 4. Promedio carrera
            // ============================
            decimal promedioCarrera = materias.Any()
                ? materias.Average(x => x.PromedioMateria ?? 0m)
                : 0m;


            // ============================
            // 5. Rendimiento por carrera
            // ============================
            List<RendimientoCarreraDto>? rendimientoCarrera = null;

            if (!string.IsNullOrEmpty(termino))
            {
                rendimientoCarrera = metricasPeriodo
                    .GroupBy(x => new { x.CodCarrera, x.NombreCarrera })
                    .Select(g => new RendimientoCarreraDto
                    {
                        CodCarrera = g.Key.CodCarrera,
                        NombreCarrera = g.Key.NombreCarrera,
                        Aprobados = g.Sum(x => x.CantidadAprobados),
                        Reprobados = g.Sum(x => x.CantidadReprobados)
                    })
                    .ToList();
            }
            else
            {
                rendimientoCarrera = metricasAnio
                    .GroupBy(x => new { x.CodCarrera, x.NombreCarrera })
                    .Select(g => new RendimientoCarreraDto
                    {
                        CodCarrera = g.Key.CodCarrera,
                        NombreCarrera = g.Key.NombreCarrera,
                        Aprobados = g.Sum(x => x.CantidadAprobados),
                        Reprobados = g.Sum(x => x.CantidadReprobados)
                    })
                    .ToList();
            }
            
            var evolucionIngresos = metricasPeriodo
                .OrderBy(x => x.Anio)
                .ThenBy(x => x.Termino)
                .ToList();

            var evolucionIngresosDto = new EvolucionIngresosDto
            {
                Periodos = evolucionIngresos.Select(x => x.NombrePeriodo).ToList(),
                CantidadIngresos = evolucionIngresos
                    .Select(x => x.TotalIngresoAdm + x.TotalIngresoCambio)
                    .ToList()
            };

            // ============================
            // 7. Materias con métricas
            // ============================
            var materiasMayorReprobacion = materias
                .GroupBy(x => new { x.CodMateria, x.NombreMateria })
                .Select(g => new MateriaReprobacionDto
                {
                    CodMateria = g.Key.CodMateria,
                    NombreMateria = g.Key.NombreMateria,
                    Reprobados = g.Sum(x => x.CantidadReprobados)
                })
                .OrderByDescending(x => x.Reprobados)
                .Take(5)
                .ToList();

            var materiasMasPobladas = materias
                .GroupBy(x => new { x.CodMateria, x.NombreMateria })
                .Select(g => new MateriaPobladaDto
                {
                    CodMateria = g.Key.CodMateria,
                    NombreMateria = g.Key.NombreMateria,
                    CantidadInscritos = g.Sum(x => x.CantidadInscripciones)
                })
                .OrderByDescending(x => x.CantidadInscritos)
                .Take(5)
                .ToList();

            // ============================
            // 8. Construcción Dashboard
            // ============================
            var dashboard = new DashboardDto
            {
                CantidadEstudiantes = totalEstudiantes,
                CantidadInscripciones = totalInscripciones,
                TasaReprobacion = tasaReprobacion,
                TotalReprobados = totalReprobados,
                TotalGraduados = totalGraduados,
                PromedioCarrera = Math.Round(promedioCarrera, 2),
                RendimientoCarrera = rendimientoCarrera,
                EvolucionIngresos = evolucionIngresosDto,
                MateriasMayorReprobacion = materiasMayorReprobacion,
                MateriasMasPobladas = materiasMasPobladas
            };

            return dashboard;
        }
    }
}
