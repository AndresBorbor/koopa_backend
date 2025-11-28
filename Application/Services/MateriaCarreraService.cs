using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services;

public class MateriaCarreraService
{
    private readonly IMateriaCarreraRepository _repository;

    public MateriaCarreraService(IMateriaCarreraRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> ListarMateriaCarrera()
    {
        return await _repository.GetAllAsync();
    }
}
