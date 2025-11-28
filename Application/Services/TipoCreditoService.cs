using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services;

public class TipoCreditoService
{
    private readonly ITipoCreditoRepository _repository;

    public TipoCreditoService(ITipoCreditoRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> ListarTipoCredito()
    {
        return await _repository.GetAllAsync();
    }
}
