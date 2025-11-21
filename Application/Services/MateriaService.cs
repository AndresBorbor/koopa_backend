using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services;

public class MateriaService
{
    private readonly IMateriaRepository _repo;

    public MateriaService(IMateriaRepository repo)
    {
        _repo = repo;
    }

    public async Task<object> ListarMaterias()
    {
        return await _repo.GetAllAsync();
    }
}
