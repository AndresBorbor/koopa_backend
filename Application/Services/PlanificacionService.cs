using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services;

public class PlanificacionService
{
    private readonly IPlanificacionRepository _repository;

    public PlanificacionService(IPlanificacionRepository repository)
    {
        _repository = repository;
    }

    public async Task<object> ListarPlanificacion()
    {
        return await _repository.GetAllAsync();
    }
}
