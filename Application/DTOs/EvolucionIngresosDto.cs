using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


public class EvolucionIngresosDto
    {
        [JsonPropertyName("periodos")]
        public List<string> Periodos { get; set; } = new();

        [JsonPropertyName("cantidad_ingresos")]
        public List<int> CantidadIngresos { get; set; } = new();
    }