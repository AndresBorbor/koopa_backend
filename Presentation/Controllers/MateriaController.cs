using KoopaBackend.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KoopaBackend.Presentation.Controllers;

[ApiController]
[Route("api/materias")]
public class MateriaController : ControllerBase
{
    private readonly MateriaService _service;

    public MateriaController(MateriaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var data = await _service.ListarMaterias();
        return Ok(data);
    }
}
