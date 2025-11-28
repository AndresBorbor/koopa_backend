using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class SemestreRepository : ISemestreRepository
    {
        private readonly KoopaDbContext _context;

        public SemestreRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Semestre>> GetAllAsync()
        {
            return await _context.Semestres.ToListAsync();
        }


    }
}