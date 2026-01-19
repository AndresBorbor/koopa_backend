using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using KoopaBackend.Application.DTOs; // Asegúrate de importar donde está el DTO

namespace KoopaBackend.Domain.Interfaces
{
    public interface IFiltroRepository
    {
        // CAMBIO: Ahora recibe anio y termino en lugar de codPeriodo directo
        Task<Dictionary<string,IEnumerable<FiltroDto>>>  GetFiltros();
    }
}