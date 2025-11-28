using System.Collections.Generic;
using System.Threading.Tasks;
using KoopaBackend.Domain.Entities; // 👈 Usamos el namespace correcto

// 👇 CAMBIO IMPORTANTE: KoopaBackend
namespace KoopaBackend.Domain.Interfaces 
{
    public interface IInscripcionesRepository
    {
        Task<IEnumerable<Inscripciones>> GetAllAsync();
    }
}