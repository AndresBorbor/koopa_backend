using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;
using KoopaBackend.Application.DTOs;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class MateriaRepository : IMateriaRepository
    {
        private readonly KoopaDbContext _context;

        public MateriaRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public Task<List<VW_MetricasMateria>> ObtenerMetricasMateriasAsync(
            int codCarrera,
            int? anio,
            string? termino)
        {
            var query = _context.VwMetricasMaterias
                .AsNoTracking()
                .Where(x => x.CodCarrera == codCarrera);

            if (anio.HasValue)
                query = query.Where(x => x.Anio == anio.Value);

            if (!string.IsNullOrEmpty(termino))
                query = query.Where(x => x.Termino.ToLower() == termino.ToLower());

            return query.ToListAsync();
        }

        public Task<List<Requisito>> ObtenerRequisitosAsync()
        {
            return _context.Requisitos
                .AsNoTracking()
                .ToListAsync();
        }
    }

}