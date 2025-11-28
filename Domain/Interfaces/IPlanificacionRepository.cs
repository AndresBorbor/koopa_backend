using System.Collections.Generic;
using System.Threading.Tasks;
using KoopaBackend.Domain.Entities;

namespace KoopaBackend.Domain.Interfaces
{
    public interface IPlanificacionRepository
    {
        Task<IEnumerable<Planificacion>> GetAllAsync();
    }
}