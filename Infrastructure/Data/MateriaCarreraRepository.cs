using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class MateriaCarreraRepository : IMateriaCarreraRepository
    {
        private readonly KoopaDbContext _context;

        public MateriaCarreraRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MateriaCarrera>> GetAllAsync()
        {
            return await _context.MateriasCarrera.ToListAsync();

        }

    }
}