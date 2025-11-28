using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services;

public class EstudianteService
{
    private readonly IEstudianteRepository _repository;

    public EstudianteService(IEstudianteRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> ListarEstudiante()
    {
        return await _repository.GetAllAsync();
    }
}
