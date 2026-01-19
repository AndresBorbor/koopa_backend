using Microsoft.AspNetCore.Mvc;
using KoopaBackend.Application.Services;
using System.Threading.Tasks;
using System;

namespace KoopaBackend.Presentation.Controllers
{
    [Route("api/filtros")]
    [ApiController]
    public class FiltroController : ControllerBase
    {
        private readonly FiltroService _service;

        public FiltroController(FiltroService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltros()
        {
            try
            {
                var resultado = await _service.GetFiltros();
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // En producción: _logger.LogError(ex, "Error en metricas");
                return StatusCode(500, new { error = "Error interno: " + ex.Message });
            }
        }
    }
}