using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;
using KoopaBackend.Domain.Entities;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class MetricasRepository : IMetricasRepository
    {
        private readonly KoopaDbContext _context;

        public MetricasRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> ObtenerMetricasAsync(
            int? codCarrera,
            int? anio,
            string? termino)
        {
            int totalEstudiantes = 0;
            int totalReprobados = 0;
            int totalInscripciones = 0;
            List<RendimientoCarreraDto>? rendimientoCarrera = null;

            // ============================
            // LOGICA DUPLICADA (ANTES queryGeneral)
            // ============================
            if (termino != null)
            {
                var queryGeneral = _context.VwMetricasCarreraPeriodos.AsNoTracking();

                if (anio.HasValue)
                    queryGeneral = queryGeneral.Where(x => x.Anio == anio.Value);

                if (codCarrera.HasValue)
                    queryGeneral = queryGeneral.Where(x => x.CodCarrera == codCarrera.Value);

                if (!string.IsNullOrEmpty(termino))
                    queryGeneral = queryGeneral.Where(x => x.Termino.ToLower() == termino.ToLower());

                totalEstudiantes = await queryGeneral.SumAsync(x => x.CantidadEstudiantes);
                totalReprobados = await queryGeneral.SumAsync(x => x.CantidadReprobados);
                totalInscripciones = await queryGeneral.SumAsync(x => x.CantidadInscripciones);

                rendimientoCarrera = await queryGeneral
                    .GroupBy(x => new { x.CodCarrera, x.NombreCarrera })
                    .Select(g => new RendimientoCarreraDto
                    {
                        CodCarrera = g.Key.CodCarrera,
                        NombreCarrera = g.Key.NombreCarrera,
                        Aprobados = g.Sum(x => x.CantidadAprobados),
                        Reprobados = g.Sum(x => x.CantidadReprobados)
                    })
                    .Where(r => r.CodCarrera != 999)
                    .ToListAsync();
            }
            else
            {
                var queryGeneral = _context.VwMetricasCarreraAnios.AsNoTracking();

                if (anio.HasValue)
                    queryGeneral = queryGeneral.Where(x => x.Anio == anio.Value);

                if (codCarrera.HasValue)
                    queryGeneral = queryGeneral.Where(x => x.CodCarrera == codCarrera.Value);

                totalEstudiantes = await queryGeneral.SumAsync(x => x.CantidadEstudiantes);
                totalReprobados = await queryGeneral.SumAsync(x => x.CantidadReprobados);
                totalInscripciones = await queryGeneral.SumAsync(x => x.CantidadInscripciones);


                rendimientoCarrera = await queryGeneral
                    .GroupBy(x => new { x.CodCarrera, x.NombreCarrera })
                    .Select(g => new RendimientoCarreraDto
                    {
                        CodCarrera = g.Key.CodCarrera,
                        NombreCarrera = g.Key.NombreCarrera,
                        Aprobados = g.Sum(x => x.CantidadAprobados),
                        Reprobados = g.Sum(x => x.CantidadReprobados)
                    })
                    .Where(r => r.CodCarrera != 999)
                    .ToListAsync();
            }

            // ============================
            // LOGICA COMUN (SIN DUPLICAR)
            // ============================

            var tasaReprobacion = 0.0;
            if (totalInscripciones > 0)
            {
                tasaReprobacion = Math.Round(
                    (double)totalReprobados * 100.0 / totalInscripciones, 2);
            }

            // Graduados
            var queryGraduados = _context.Graduados.AsQueryable();

            if (anio.HasValue)
                queryGraduados = queryGraduados.Where(g => g.Anio == anio.Value);

            if (codCarrera.HasValue)
                queryGraduados = queryGraduados.Where(g => g.CodCarrera == codCarrera.Value);

            var totalGraduados = await queryGraduados
                .SumAsync(g => g.CantidadGraduados);
            

            // Promedio carrera
            var queryMateria = _context.VwMetricasMaterias.AsNoTracking();

            if (anio.HasValue)
                queryMateria = queryMateria.Where(x => x.Anio == anio.Value);

            if (codCarrera.HasValue)
                queryMateria = queryMateria.Where(x => x.CodCarrera == codCarrera.Value);

            if (!string.IsNullOrEmpty(termino))
                    queryMateria = queryMateria.Where(x => x.Termino.ToLower() == termino.ToLower());

            var promedioCarreraFacultad =
                await queryMateria.AverageAsync(x => (double?)x.PromedioMateria) ?? 0;

            // Evolución de ingresos
            var periodos = await _context.VwMetricasCarreraPeriodos
                .Where(i => !codCarrera.HasValue || i.CodCarrera == codCarrera.Value)
                .OrderBy(i => i.Anio)
                .ThenBy(i => i.Termino)
                .Select(i => i.NombrePeriodo)
                .ToListAsync();

            var cantidadIngresos = await _context.VwMetricasCarreraPeriodos
                .Where(i => !codCarrera.HasValue || i.CodCarrera == codCarrera.Value)
                .OrderBy(i => i.Anio)
                .ThenBy(i => i.Termino)
                .Select(i => i.TotalIngresoAdm + i.TotalIngresoCambio)
                .ToListAsync();

            var evolucionIngresosDto = new EvolucionIngresosDto
            {
                Periodos = periodos,
                CantidadIngresos = cantidadIngresos
            };

            // Materias con mayor reprobación
            var materiasMayorReprobacion = await queryMateria
                .GroupBy(x => new { x.CodMateria, x.NombreMateria })
                .Select(g => new MateriaReprobacionDto
                {
                    // Si CodMateria es nulable, usamos ?? 0 para que encaje en el int del DTO
                    CodMateria = g.Key.CodMateria,
                    
                    NombreMateria = g.Key.NombreMateria ?? "Sin Nombre",

                    // Sumamos y convertimos el resultado nulable a int (si es null, será 0)
                    Reprobados = (int)(g.Sum(x => (int?)x.CantidadReprobados) ?? 0)
                })
                .OrderByDescending(x => x.Reprobados)
                .Take(5)
                .ToListAsync();

            // Materias más pobladas
            var materiasMasPobladas = await queryMateria
                .GroupBy(x => new { x.CodMateria, x.NombreMateria })
                .Select(g => new MateriaPobladaDto
                {
                    CodMateria = g.Key.CodMateria,
                    NombreMateria = g.Key.NombreMateria,
                    CantidadInscritos = g.Sum(x => x.CantidadInscripciones)
                })
                .OrderByDescending(x => x.CantidadInscritos)
                .Take(5)
                .ToListAsync();

            // Dashboard
            var dashboard = new DashboardDto
            {
                CantidadEstudiantes = totalEstudiantes,
                CantidadInscripciones = totalInscripciones,
                TasaReprobacion = tasaReprobacion,
                TotalReprobados = totalReprobados,
                TotalGraduados = totalGraduados,
                PromedioCarrera = Math.Round(promedioCarreraFacultad, 2),
                RendimientoCarrera = rendimientoCarrera,
                EvolucionIngresos = evolucionIngresosDto,
                MateriasMayorReprobacion = materiasMayorReprobacion,
                MateriasMasPobladas = materiasMasPobladas
            };

            return dashboard;
        }

    }
}
