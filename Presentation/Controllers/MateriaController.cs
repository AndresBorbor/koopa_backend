using KoopaBackend.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace KoopaBackend.API.Controllers
{
    [Route("api/materias")]
    [ApiController]
    public class MateriaController : ControllerBase
    {
        private readonly MateriaService _materiaService;

        public MateriaController(MateriaService materiaService)
        {
            _materiaService = materiaService;
        }

        [HttpGet("malla-stats")]
        public async Task<IActionResult> ObtenerMalla(
            [FromQuery] int codCarrera,
            [FromQuery] int? anio,
            [FromQuery] string? termino)
        {
            try
            {
                if (codCarrera <= 0)
                    return BadRequest("El código de carrera es obligatorio.");

                var resultado = await _materiaService
                    .ObtenerMallaAsync(codCarrera, anio, termino);

                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
