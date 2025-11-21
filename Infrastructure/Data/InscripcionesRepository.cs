using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Infrastructure.Data;

public class InscripcionesRepository : IInscripcionesRepository
{
    // Datos fake para probar
    private static readonly List<Inscripciones> _data = new()
    {
        new Inscripciones { Id = 1, Nombre = "Joseph" },
        new Inscripciones { Id = 2, Nombre = "Keyla" }
    };

    public Task<IEnumerable<Inscripciones>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Inscripciones>>(_data);
    }
}
