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

        // 1. Obtener todas las carreras
        public async Task<IEnumerable<Carrera>> GetAllAsync()
        {
            // CORRECCIÓN: Usamos _context.Carreras (Plural, como está en el DbContext)
            return await _context.Carreras.ToListAsync();
        }

        
    }
}