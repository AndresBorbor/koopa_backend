using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("GRADUADOS", Schema = "dbo")]
    public class Graduados
    {
        [Key]
        [Column("COD_CARRERA")]
        public int CodCarrera { get; set; }

        [Column("ANIO")]
        public int Anio { get; set; }

        [Column("CANTIDAD_GRADUADOS")]
        public int CantidadGraduados { get; set; }
    }
}