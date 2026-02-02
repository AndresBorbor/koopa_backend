using System.Collections.Generic;
using System.Threading.Tasks;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Application.DTOs;

namespace KoopaBackend.Domain.Interfaces
{
    public interface IMateriaRepository
    {
        Task<List<VW_MetricasMateria>> ObtenerMetricasMateriasAsync(
            int codCarrera,
            int? anio,
            string? termino);

        Task<List<Requisito>> ObtenerRequisitosAsync();
    }

}