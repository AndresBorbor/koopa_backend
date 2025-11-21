using KoopaBackend.Domain.Entities;

namespace KoopaBackend.Domain.Interfaces;

public interface IMateriaRepository
{
    Task<IEnumerable<Materia>> GetAllAsync();
}
