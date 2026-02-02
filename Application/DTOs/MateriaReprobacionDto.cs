using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

public class MateriaReprobacionDto
    {
        [JsonPropertyName("cod_materia")]
        public int CodMateria { get; set; }
        
        [JsonPropertyName("nombre_materia")]
        public string NombreMateria { get; set; } = string.Empty;

        [JsonPropertyName("reprobados")]
        public int Reprobados { get; set; }
    }