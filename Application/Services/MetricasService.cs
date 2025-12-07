using KoopaBackend.Domain.Interfaces;
using System.Threading.Tasks;
using System;

namespace KoopaBackend.Application.Services
{
    public class MetricasService
    {
        private readonly IMetricasRepository _repository;

        public MetricasService(IMetricasRepository repository)
        {
            _repository = repository;
        }

        public async Task<DashboardDto> ObtenerDashboardAsync(int? codCarrera, int? anio, string? termino)
        {
            // 1. Validaciones de Negocio
            if (anio.HasValue && (anio < 1900 || anio > 3000))
            {
                throw new ArgumentException("El año proporcionado no es válido.");
            }

            // 2. Llamada al Repositorio
            return await _repository.ObtenerMetricasAsync(codCarrera, anio, termino);
        }
    }
}