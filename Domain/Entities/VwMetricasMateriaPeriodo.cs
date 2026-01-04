using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    public class VwMetricasMateriaPeriodo
    {
        
        public int Anio { get; set; }
        public string Termino { get; set; }
        public int CodCarrera { get; set; }
        public string NombreCarrera { get; set; }
        public int CodMateria { get; set; }
        public string NombreMateria { get; set; }
        public int Inscritos { get; set; }
        public int Reprobados { get; set; }
        public int Aprobados { get; set; }
        public decimal PromedioMateria { get; set; }
    }
    
}
