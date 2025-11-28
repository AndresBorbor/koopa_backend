using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data; // Donde está tu KoopaDbContext

namespace KoopaBackend.Infrastructure.Repositories
{
    public class InscripcionesRepository : IInscripcionesRepository
    {
        private readonly KoopaDbContext _context;

        public InscripcionesRepository(KoopaDbContext context)
        {
            _context = context;
        }

        // 1. Implementación de GetAllAsync
        public async Task<IEnumerable<Inscripciones>> GetAllAsync()
        {
            // Retorna los datos reales de DB2
            return await _context.Inscripciones.ToListAsync();

        }

        // // 2. Implementación de GetByEstudianteAsync
        // public async Task<IEnumerable<Inscripciones>> GetByEstudianteAsync(long codEstudiante)
        // {
        //     return await _context.Inscripciones
        //                          .Where(x => x.CodEstudiante == codEstudiante)
        //                          .ToListAsync();
        // }
    }
}