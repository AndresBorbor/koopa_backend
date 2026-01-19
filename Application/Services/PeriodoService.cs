using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services;

public class PeriodoService
{
    private readonly IPeriodoRepository _repository;

    public PeriodoService(IPeriodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> ListarPeriodo()
    {
        return await _repository.GetAllAsync();
    }
}
