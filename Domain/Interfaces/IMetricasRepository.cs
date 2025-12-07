using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KoopaBackend.Domain.Interfaces
{
    public interface IMetricasRepository
    {
        // CAMBIO: Ahora recibe anio y termino en lugar de codPeriodo directo
        Task<DashboardDto> ObtenerMetricasAsync(int? codCarrera, int? anio, string? termino);
    }
    public class MateriaPobladaDto
    {
        public string NombreMateria { get; set; }
        public int CantidadInscritos { get; set; }
    }

    public class DashboardDto
    {
        [JsonPropertyName("cant_estudiantes")]
        public int CantEstudiantes { get; set; }

        [JsonPropertyName("tasa_reprobacion")]
        public double TasaReprobacion { get; set; }

        [JsonPropertyName("total_reprobados")]
        public int TotalReprobados { get; set; }

        [JsonPropertyName("tasa_graduados")]
        public double TasaGraduados { get; set; }

        [JsonPropertyName("total_graduados")]
        public int TotalGraduados { get; set; }

        [JsonPropertyName("promedio_carrera")]
        public double PromedioCarrera { get; set; }

        [JsonPropertyName("rendimiento_carrera")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<RendimientoCarreraDto>? RendimientoCarrera { get; set; }

        [JsonPropertyName("evolucion_ingresos")]
        public EvolucionIngresosDto EvolucionIngresos { get; set; }

        [JsonPropertyName("materias_mayor_reprobacion")]
        public List<MateriaReprobacionDto> MateriasMayorReprobacion { get; set; }

        [JsonPropertyName("materias_mas_pobladas")]
        public List<MateriaPobladaDto> MateriasMasPobladas { get; set; }
    }

    public class RendimientoCarreraDto
    {
        [JsonPropertyName("nombre_carrera")]
        public string NombreCarrera { get; set; } = string.Empty;

        [JsonPropertyName("reprobados")]
        public int Reprobados { get; set; }

        [JsonPropertyName("aprobados")]
        public int Aprobados { get; set; }
    }

    public class EvolucionIngresosDto
    {
        [JsonPropertyName("periodos")]
        public List<string> Periodos { get; set; } = new();

        [JsonPropertyName("cantidad_ingresos")]
        public List<int> CantidadIngresos { get; set; } = new();
    }

    public class MateriaReprobacionDto
    {
        [JsonPropertyName("nombre_materia")]
        public string NombreMateria { get; set; } = string.Empty;

        [JsonPropertyName("reprobados")]
        public int Reprobados { get; set; }
    }
}