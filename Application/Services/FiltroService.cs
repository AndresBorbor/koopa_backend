using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Application.DTOs; // Asegúrate de importar tus DTOs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KoopaBackend.Application.Services
{
    public class FiltroService
    {
        private readonly IFiltroRepository _repository;

        public FiltroService(IFiltroRepository repository)
        {
            _repository = repository;
        }

        public async Task<Dictionary<string,IEnumerable<FiltroDto>>> GetFiltros()
        {
             return await _repository.GetFiltros();
        }
    }
}