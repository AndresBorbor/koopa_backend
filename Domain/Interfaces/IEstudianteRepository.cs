using System.Collections.Generic;
using System.Threading.Tasks;
using KoopaBackend.Domain.Entities;

namespace KoopaBackend.Domain.Interfaces
{
    public interface IEstudianteRepository
    {

        Task<IEnumerable<Estudiante>> GetAllAsync();
    }
}