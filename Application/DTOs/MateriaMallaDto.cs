using System.Collections.Generic;

namespace KoopaBackend.Application.DTOs
{
    public class MateriaMallaDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string CodigoMateria { get; set; }
        public string Nivel { get; set; }
        public string Color { get; set; }
        
        // 👇 ESTE ES EL CAMPO QUE DABA ERROR CS0117
        public int CantidadEstudiantes { get; set; } 
        public int CantidadInscripciones { get; set; } 
        
        public string Estado { get; set; }

        // 👇 ESTE ES EL CAMPO QUE DABA ERROR CS0029 (Ahora es Dictionary)
        public Dictionary<string, StatsMallaDto> Stats { get; set; } = new Dictionary<string, StatsMallaDto>();

        // 👇 ESTOS DABAN ERROR CS0117
        public List<string> PreRequisitos { get; set; } = new List<string>();
        public List<string> CoRequisitos { get; set; } = new List<string>();
        public decimal? PromedioMateria { get; set; }
        public List<RendimientoMallaDto> Rendimiento { get; set; } = new List<RendimientoMallaDto>();
    }

    public class StatsMallaDto
    {
        public int Inscritos { get; set; }
        public int Reprobados { get; set; }
        public double? ReprobadosPorcentaje { get; set; }
        public int Habilitados { get; set; }
        public string Descripcion { get; set; }
        public string NotaPie { get; set; }
    }

    public class RendimientoMallaDto
    {
        public string Periodo { get; set; }
        public int Inscripciones { get; set; }
        public int Reprobados { get; set; }
    }
}