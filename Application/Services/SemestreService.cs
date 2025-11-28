using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services;

public class SemestreService
{
    private readonly ISemestreRepository _repository;

    public SemestreService(ISemestreRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> ListarSemestre()
    {
        return await _repository.GetAllAsync();
    }
}
