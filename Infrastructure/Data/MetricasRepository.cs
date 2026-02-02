using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class MetricasRepository : IMetricasRepository
    {
        private readonly KoopaDbContext _context;

        public MetricasRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public Task<List<VW_MetricasCarreraPeriodo>> ObtenerMetricasCarreraPeriodosAsync(
            int? codCarrera,
            int? anio,
            string? termino)
        {
            var query = _context.VwMetricasCarreraPeriodos
                .AsNoTracking()
                .AsQueryable();

            if (codCarrera.HasValue)
                query = query.Where(x => x.CodCarrera == codCarrera.Value);

            if (anio.HasValue)
                query = query.Where(x => x.Anio == anio.Value);

            if (!string.IsNullOrEmpty(termino))
                query = query.Where(x => x.Termino.ToLower() == termino.ToLower());

            return query.ToListAsync();
        }

        public Task<List<VW_MetricasCarreraAnio>> ObtenerMetricasCarreraAniosAsync(
            int? codCarrera,
            int? anio)
        {
            var query = _context.VwMetricasCarreraAnios
                .AsNoTracking()
                .AsQueryable();

            if (codCarrera.HasValue)
                query = query.Where(x => x.CodCarrera == codCarrera.Value);

            if (anio.HasValue)
                query = query.Where(x => x.Anio == anio.Value);

            return query.ToListAsync();
        }

        public Task<List<VW_MetricasMateria>> ObtenerMetricasMateriasAsync(
            int? codCarrera,
            int? anio,
            string? termino)
        {
            var query = _context.VwMetricasMaterias
                .AsNoTracking()
                .AsQueryable();

            if (codCarrera.HasValue)
                query = query.Where(x => x.CodCarrera == codCarrera.Value);

            if (anio.HasValue)
                query = query.Where(x => x.Anio == anio.Value);

            if (!string.IsNullOrEmpty(termino))
                query = query.Where(x => x.Termino.ToLower() == termino.ToLower());

            return query.ToListAsync();
        }

        public Task<List<Graduados>> ObtenerGraduadosAsync(
            int? codCarrera,
            int? anio)
        {
            var query = _context.Graduados.AsQueryable();

            if (codCarrera.HasValue)
                query = query.Where(x => x.CodCarrera == codCarrera.Value);

            if (anio.HasValue)
                query = query.Where(x => x.Anio == anio.Value);

            return query.ToListAsync();
        }
    }
}
