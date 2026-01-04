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
        public async Task<DashboardDto> ObtenerMetricasAsync(
    int? codCarrera,
    int? anio,
    string? termino)
        {
            var query = _context.VwMetricasMateriaPeriodos.AsNoTracking();
            

            if (anio.HasValue)
                query = query.Where(x => x.Anio == anio.Value);

            if (!string.IsNullOrEmpty(termino))
                query = query.Where(x => x.Termino == termino);

            if (codCarrera.HasValue)
                query = query.Where(x => x.CodCarrera == codCarrera.Value);
            
            var dashboard = new DashboardDto();

            
            dashboard.CantEstudiantes = await query.SumAsync(x => x.Inscritos);
            dashboard.TotalReprobados = await query.SumAsync(x => x.Reprobados);
            dashboard.TasaReprobacion = dashboard.CantEstudiantes > 0
                ? Math.Round(dashboard.TotalReprobados * 100.0 / dashboard.CantEstudiantes, 2)
                : 0;
            dashboard.PromedioCarrera = await query.AverageAsync(x => (double?)x.PromedioMateria) ?? 0;
            // 2️⃣ Rendimiento por carrera (solo si no filtramos por carrera)


            if (!codCarrera.HasValue)
            {
                dashboard.RendimientoCarrera = await query
                    .GroupBy(x => x.NombreCarrera)
                    .Select(g => new RendimientoCarreraDto
                    {
                        NombreCarrera = g.Key,
                        Aprobados = g.Sum(x => x.Aprobados),
                        Reprobados = g.Sum(x => x.Reprobados)
                    })
                    .ToListAsync();
            }

            // 3️⃣ Materias con mayor reprobación (TOP 5)
            dashboard.MateriasMayorReprobacion = await query
                .GroupBy(x => x.NombreMateria)
                .Select(g => new MateriaReprobacionDto
                {
                    NombreMateria = g.Key,
                    Reprobados = g.Sum(x => x.Reprobados)
                })
                .OrderByDescending(x => x.Reprobados)
                .Take(5)
                .ToListAsync();

            // 4️⃣ Materias más pobladas (TOP 5)
            dashboard.MateriasMasPobladas = await query
                .GroupBy(x => x.NombreMateria)
                .Select(g => new MateriaPobladaDto
                {
                    NombreMateria = g.Key,
                    CantidadInscritos = g.Sum(x => x.Inscritos)
                })
                .OrderByDescending(x => x.CantidadInscritos)
                .Take(5)
                .ToListAsync();
           
            // 5️⃣ Evolución de ingresos
            dashboard.EvolucionIngresos = new EvolucionIngresosDto
            {
                Periodos = await _context.Ingresos
                    .Where(i => !codCarrera.HasValue || i.CodCarrera == codCarrera.Value)
                    .OrderBy(i => i.Anio).ThenBy(i => i.Termino)
                    .Select(i => i.Anio.ToString() +i.Termino.ToString())
                    .ToListAsync(),

                CantidadIngresos = await _context.Ingresos
                    .Where(i => !codCarrera.HasValue || i.CodCarrera == codCarrera.Value)
                    .OrderBy(i => i.Anio).ThenBy(i => i.Termino)
                    .Select(i =>  i.TotalIngresoAdm + i.TotalIngresoCambio)    
                    .ToListAsync()
            };

            // 6️⃣ Graduados
            var graduadosQuery = _context.Graduados.AsQueryable();
            if (anio.HasValue)
                graduadosQuery = graduadosQuery.Where(g => g.Anio == anio.Value);
            if (codCarrera.HasValue)
                graduadosQuery = graduadosQuery.Where(g => g.CodCarrera == codCarrera.Value);

            dashboard.TotalGraduados = await graduadosQuery.SumAsync(g => g.CantidadGraduados);

            // Tasa de graduados sobre inscritos
            dashboard.TasaGraduados = dashboard.CantEstudiantes > 0
                ? Math.Round(dashboard.TotalGraduados * 100.0 / dashboard.CantEstudiantes, 2)
                : 0;

            return dashboard;
        }

    }
}