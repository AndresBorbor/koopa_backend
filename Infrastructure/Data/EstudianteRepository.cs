using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class EstudianteRepository : IEstudianteRepository
    {
        private readonly KoopaDbContext _context;

        public EstudianteRepository(KoopaDbContext context)
        {
            _context = context;
        }

        // 1. Obtener todas las Estudiantes
        public async Task<IEnumerable<Estudiante>> GetAllAsync()
        {
            // CORRECCIÓN: Usamos _context.Estudiantes (Plural, como está en el DbContext)
            return await _context.Estudiantes.ToListAsync();
        }
    }
}