using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Infrastructure.Data;

public class MateriaRepository : IMateriaRepository
{
    public async Task<IEnumerable<Materia>> GetAllAsync()
    {
        // Luego esto vendrá de DB2. Por ahora solo devuelve algo de prueba:
        return await Task.FromResult(new List<Materia>
        {
            new Materia { Id = 1, Nombre = "Koopa Verde" },
            new Materia { Id = 2, Nombre = "Koopa Rojo" }
        });
    }
}
