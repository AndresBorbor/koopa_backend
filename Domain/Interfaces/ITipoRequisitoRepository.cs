using KoopaBackend.Domain.Entities;

namespace KoopaBackend.Domain.Interfaces
{
    public interface ITipoRequisitoRepository
    {
        Task<IEnumerable<TipoRequisito>> GetAllAsync();
        
    }
}