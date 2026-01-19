using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class PeriodoRepository : IPeriodoRepository
    {
        private readonly KoopaDbContext _context;

        public PeriodoRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Periodo>> GetAllAsync()
        {
            return await _context.Periodos.ToListAsync();
        }
    }
}