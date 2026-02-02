using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class CarreraRepository : ICarreraRepository
    {
        private readonly KoopaDbContext _context;

        public CarreraRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Carrera>> ListarCarreras()
        {
            return await _context.Carreras.ToListAsync();
        }

        
    }
}