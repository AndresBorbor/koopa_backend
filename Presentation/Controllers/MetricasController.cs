using Microsoft.AspNetCore.Mvc;
using KoopaBackend.Application.Services;
using System.Threading.Tasks;
using System;

namespace KoopaBackend.Presentation.Controllers
{
    [Route("api/metricas")]
    [ApiController]
    public class MetricasController : ControllerBase
    {
        private readonly MetricasService _service;

        public MetricasController(MetricasService service)
        {
            _service = service;
        }

        // GET: api/metricas/dashboard?anio=2025&termino=1S&codCarrera=5
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardMetrics(
            [FromQuery] int anio, 
            [FromQuery] string termino, 
            [FromQuery] int? codCarrera = null)
        {
            try
            {
                var resultado = await _service.ObtenerDashboardAsync(codCarrera, anio, termino);
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