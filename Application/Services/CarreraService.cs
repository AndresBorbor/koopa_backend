using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services;

public class CarreraService
{
    private readonly ICarreraRepository _repository;

    public CarreraService(ICarreraRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> ListarCarreras()
    {
        return await _repository.ListarCarreras();
    }
}
