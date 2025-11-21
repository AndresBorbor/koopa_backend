using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services;

public class InscripcionesService
{
    private readonly IInscripcionesRepository _repository;

    public InscripcionesService(IInscripcionesRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> ListarInscripciones()
    {
        return await _repository.GetAllAsync();
    }
}
