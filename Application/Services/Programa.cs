using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services;

public class ProgramaService
{
    private readonly IProgramaRepository _repository;

    public ProgramaService(IProgramaRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> ListarPrograma()
    {
        return await _repository.GetAllAsync();
    }
}
