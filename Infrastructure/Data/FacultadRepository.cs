using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class FacultadRepository : IFacultadRepository
    {
        private readonly KoopaDbContext _context;

        public FacultadRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Facultad>> GetAllAsync()
        {
            return await _context.Facultades.ToListAsync();

        }

    }
}