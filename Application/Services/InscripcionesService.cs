using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services;

public class InscripcionesService
{
    private readonly IInscripcionesRepository _repository;

    public InscripcionesService(IInscripcionesRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Inscripciones>> GetAllAsync()
        => _repository.GetAllAsync();
}
