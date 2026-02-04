using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class RendimientoCarreraDto
    {
        [JsonPropertyName("cod_carrera")]
        public int CodCarrera { get; set; }
        
        [JsonPropertyName("nombre_carrera")]
        public string NombreCarrera { get; set; } = string.Empty;

        [JsonPropertyName("reprobados")]
        public int Reprobados { get; set; }

        [JsonPropertyName("aprobados")]
        public int Aprobados { get; set; }
    }