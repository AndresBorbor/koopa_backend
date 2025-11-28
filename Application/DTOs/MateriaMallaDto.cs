using System.Collections.Generic;

namespace KoopaBackend.Application.DTOs // O el namespace que uses para DTOs
{
    public class MateriaMallaDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Nivel { get; set; } // Ej: "NIVEL 100"
        public string Color { get; set; } // Ej: "#22c55e"
        
        // Array de historial (Rendimiento)
        public List<RendimientoMallaDto> Rendimiento { get; set; } = new List<RendimientoMallaDto>();
        
        // Objeto de estadísticas (Stats)
        public StatsMallaDto Stats { get; set; }
    }

    public class RendimientoMallaDto
    {
        public string Periodo { get; set; }
        public int Inscripciones { get; set; }
        public int Reprobados { get; set; }
    }

    public class StatsMallaDto
    {
        public int Reprobados { get; set; }
        public double ReprobadosPorcentaje { get; set; } // Ej: 0.25
        public int AprobaronRequisitos { get; set; }
        public int Habilitados { get; set; }
        public string Descripcion { get; set; }
        public string NotaPie { get; set; }
    }
}