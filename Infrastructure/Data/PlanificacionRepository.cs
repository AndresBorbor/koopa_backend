using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data; 

namespace KoopaBackend.Infrastructure.Repositories
{
    public class PlanificacionRepository : IPlanificacionRepository
    {
        private readonly KoopaDbContext _context;

        public PlanificacionRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Planificacion>> GetAllAsync()
        {
            return await _context.Planificaciones.ToListAsync();

        }

    }
}