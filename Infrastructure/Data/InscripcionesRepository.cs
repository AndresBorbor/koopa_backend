using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data; 

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

    }
}