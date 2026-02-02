using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class RequisitoRepository : IRequisitoRepository
    {
        private readonly KoopaDbContext _context;

        public RequisitoRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Requisito>> GetAllAsync()
        {
            return await _context.Requisitos.ToListAsync();
        }

        
    }
}