using KoopaBackend.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KoopaBackend.Presentation.Controllers;

[ApiController]
[Route("api/inscripciones")]
public class InscripcionesController : ControllerBase
{
    private readonly InscripcionesService _service;

    public InscripcionesController(InscripcionesService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var data = await _service.ListarInscripciones();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        // Solo para probar que el código cambió.
        // No llama al servicio, solo devuelve algo básico.
        return Ok(new
        {
            message = "Endpoint funcionando",
            id_recibido = id
        });
    }
}


