using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class DashboardDto
    {
        [JsonPropertyName("cantidad_estudiantes")]
        public int CantidadEstudiantes { get; set; }

        [JsonPropertyName("cantidad_inscripciones")]
        public int CantidadInscripciones { get; set; }

        [JsonPropertyName("tasa_reprobacion")]
        public double TasaReprobacion { get; set; }

        [JsonPropertyName("total_reprobados")]
        public int TotalReprobados { get; set; }

        [JsonPropertyName("tasa_graduados")]
        public double TasaGraduados { get; set; }

        [JsonPropertyName("total_graduados")]
        public int TotalGraduados { get; set; }

        [JsonPropertyName("promedio_carrera")]
        public decimal PromedioCarrera { get; set; }

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
