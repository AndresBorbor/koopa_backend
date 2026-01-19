using KoopaBackend.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace KoopaBackend.Presentation.Controllers;

[Route("api/carreras")]
[ApiController]
public class CarreraController : ControllerBase
{
    private readonly CarreraService _service;

    public CarreraController(CarreraService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var data = await _service.ListarCarreras();
        return Ok(data);
    }
}