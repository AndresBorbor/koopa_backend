using KoopaBackend.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KoopaBackend.Presentation.Controllers;

[Route("api/materias")]
[ApiController]
public class MateriaController : ControllerBase
{
    private readonly MateriaService _service;

    public MateriaController(MateriaService service)
    {
        _service = service;
    }

    // GET: api/materias/malla-stats/5
    // Ahora requerimos el ID de la carrera en la URL
    [HttpGet("malla-stats/{carreraId}")]
    public async Task<IActionResult> GetMateriaMallaStats(int carreraId)
    {
        // Validación básica
        if (carreraId <= 0)
        {
            return BadRequest("El ID de la carrera debe ser un número positivo.");
        }

        try 
        {
            // Pasamos el ID al servicio
            var datos = await _service.ObtenerDatosMallaAsync(carreraId);
            Console.WriteLine("********************");
            Console.WriteLine(datos);
            Console.WriteLine("********************");
            if (datos == null || !datos.Any())
            {
                // Un 204 No Content es a veces mejor que 404 si la carrera existe pero no tiene malla cargada,
                // pero un 404 con mensaje está bien para este caso.
                return NotFound($"No se encontraron datos de malla para la carrera con ID {carreraId}.");
            }

            return Ok(datos);
        }
        catch (Exception ex)
        {
            // Manejo básico de errores por si falla la conexión a BD
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }
}