using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Application.DTOs; // Asegúrate de importar tus DTOs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KoopaBackend.Application.Services;

public class MateriaService
{
    private readonly IMateriaRepository _repository;

    public MateriaService(IMateriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<MateriaMallaDto>> ObtenerDatosMallaAsync(int codCarrera,int? anio, string? termino)
    {
        // 1. Llamamos al repositorio
        var datos = await _repository.ObtenerDatosMallaAsync(codCarrera, anio, termino);

        // (Opcional) Aquí podrías poner lógica extra si fuera necesaria.
        // Por ejemplo: Validar que la lista no venga vacía, o filtrar algo más.
        
        // 2. Retornamos al controlador
        return datos;
    }
}