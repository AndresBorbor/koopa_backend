using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    public class VW_MetricasCarreraAnio
    {
        
        public int Anio { get; set; }
        public int CodCarrera { get; set; }
        public string NombreCarrera { get; set; }
        public int CantidadEstudiantes { get; set; }
        public int CantidadInscripciones { get; set; }
        public int CantidadAprobados { get; set; }
        public int CantidadReprobados { get; set; }
        public int CantidadGraduados { get; set; }
        public int TotalIngresoAdm { get; set; }
        public int TotalIngresoCambio { get; set; }
    }
    
}
