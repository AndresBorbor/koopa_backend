using KoopaBackend.Domain.Entities;

namespace KoopaBackend.Domain.Interfaces;

public interface IInscripcionesRepository
{
    Task<IEnumerable<Inscripciones>> GetAllAsync();
}
