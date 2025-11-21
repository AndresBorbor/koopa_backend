using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Infrastructure.Data;

public class InscripcionesRepository : IInscripcionesRepository
{
    public async Task<IEnumerable<Inscripciones>> GetAllAsync()
    {
        // Luego esto vendrá de DB2. Por ahora solo devuelve algo de prueba:
        return await Task.FromResult(new List<Inscripciones>
        {
            new Inscripciones { Id = 1, Nombre = "Koopa Verde" },
            new Inscripciones { Id = 2, Nombre = "Koopa Rojo" }
        });
    }
}
