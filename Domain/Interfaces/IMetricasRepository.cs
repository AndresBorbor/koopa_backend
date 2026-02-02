using KoopaBackend.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using KoopaBackend.Domain.Entities;

namespace KoopaBackend.Domain.Interfaces
{
    public interface IMetricasRepository
    {
        Task<List<VW_MetricasCarreraPeriodo>> ObtenerMetricasCarreraPeriodosAsync(int? codCarrera, int? anio, string? termino);
        Task<List<VW_MetricasCarreraAnio>> ObtenerMetricasCarreraAniosAsync(int? codCarrera, int? anio);
        Task<List<VW_MetricasMateria>> ObtenerMetricasMateriasAsync(int? codCarrera, int? anio, string? termino);
        Task<List<Graduados>> ObtenerGraduadosAsync(int? codCarrera, int? anio);
    }
}
