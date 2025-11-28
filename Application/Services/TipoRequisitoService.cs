using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services;

public class TipoRequisitoService
{
    private readonly ITipoRequisitoRepository _repository;

    public TipoRequisitoService(ITipoRequisitoRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> ListarTipoRequisito()
    {
        return await _repository.GetAllAsync();
    }
}
