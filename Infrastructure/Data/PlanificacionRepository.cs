using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data; // Donde está tu KoopaDbContext

namespace KoopaBackend.Infrastructure.Repositories
{
    public class PlanificacionRepository : IPlanificacionRepository
    {
        private readonly KoopaDbContext _context;

        public PlanificacionRepository(KoopaDbContext context)
        {
            _context = context;
        }

        // 1. Implementación de GetAllAsync
        public async Task<IEnumerable<Planificacion>> GetAllAsync()
        {
            // Retorna los datos reales de DB2
            return await _context.Planificaciones.ToListAsync();

        }

    }
}