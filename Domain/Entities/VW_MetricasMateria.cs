using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    public class VW_MetricasMateria
    {
        
        public string NombrePeriodo { get; set; }
        public int Anio { get; set; }
        public string Termino { get; set; }
        public int CodCarrera { get; set; }
        public string NombreCarrera { get; set; }
        public int CodMateria { get; set; }
        public string NombreMateria { get; set; }
        public int CantidadEstudiantes { get; set; }
        public int CantidadInscripciones { get; set; }
        public int CantidadAprobados { get; set; }
        public int CantidadReprobados { get; set; }
        public string NivelCarrera { get; set; }
        public int CodTipoCredito { get; set; }
        public float PromedioMateria { get; set; }
    }
    
}
