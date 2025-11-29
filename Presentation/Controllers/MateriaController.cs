using KoopaBackend.Application.Services; // Asegúrate de que aquí esté tu MateriaService
using Microsoft.AspNetCore.Mvc;

namespace KoopaBackend.Presentation.Controllers;

[Route("api/materias")]
[ApiController]
public class MateriaController : ControllerBase
{
    // Cambiamos IMateriaRepository por MateriaService
    private readonly MateriaService _service;

    public MateriaController(MateriaService service)
    {
        _service = service;
    }

    // GET: api/materias/malla-stats
    [HttpGet("malla-stats")]
    public async Task<IActionResult> GetMateriaMallaStats()
    {
        // El controlador delega la lógica al servicio
        var datos = await _service.ObtenerDatosMallaAsync();

        if (datos == null)
        {
            return NotFound("No se encontraron datos para la malla.");
        }

        return Ok(datos);
    }
}