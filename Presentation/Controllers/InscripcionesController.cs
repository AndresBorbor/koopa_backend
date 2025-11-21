using KoopaBackend.Application.Services;
using KoopaBackend.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace KoopaBackend.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InscripcionesController : ControllerBase
{
    private readonly InscripcionesService _service;

    public InscripcionesController(InscripcionesService service)
    {
        _service = service;
    }

    // GET: api/inscripciones
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Inscripciones>>> GetAll()
    {
        var data = await _service.GetAllAsync();
        return Ok(data);
    }
}
