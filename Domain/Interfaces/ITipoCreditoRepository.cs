using KoopaBackend.Domain.Entities;

namespace KoopaBackend.Domain.Interfaces
{
    public interface ITipoCreditoRepository
    {
        Task<IEnumerable<TipoCredito>> GetAllAsync();
    }
}