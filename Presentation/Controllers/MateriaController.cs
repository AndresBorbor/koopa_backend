using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using KoopaBackend.Domain.Interfaces; // Para IMateriaRepository

namespace KoopaBackend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MateriaController : ControllerBase
    {
        private readonly IMateriaRepository _materiaRepository;

        public MateriaController(IMateriaRepository materiaRepository)
        {
            _materiaRepository = materiaRepository;
        }

        // GET: api/materia/malla-stats
        [HttpGet("malla-stats")]
        public async Task<IActionResult> GetMateriaMallaStats()
        {
            // Llamamos al repositorio que ya nos devuelve el JSON listo (DTOs)
            var datos = await _materiaRepository.ObtenerDatosMallaAsync();
            
            if (datos == null)
            {
                return NotFound("No se encontraron datos para la malla.");
            }

            return Ok(datos);
        }
    }
}