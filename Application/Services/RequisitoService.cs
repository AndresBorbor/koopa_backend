using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services;

public class RequisitoService
{
    private readonly IRequisitoRepository _repository;

    public RequisitoService(IRequisitoRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> ListarRequisitos()
    {
        return await _repository.GetAllAsync();
    }
}
