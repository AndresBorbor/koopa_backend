using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class TipoCreditoRepository : ITipoCreditoRepository
    {
        private readonly KoopaDbContext _context;

        public TipoCreditoRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoCredito>> GetAllAsync()
        {
            return await _context.TiposCredito.ToListAsync();
        }
    }
}