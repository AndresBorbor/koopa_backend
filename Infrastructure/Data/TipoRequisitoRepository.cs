using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class TipoRequisitoRepository : ITipoRequisitoRepository
    {
        private readonly KoopaDbContext _context;

        public TipoRequisitoRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoRequisito>> GetAllAsync()
        {
            return await _context.TiposRequisito.ToListAsync();

        }
    }
}