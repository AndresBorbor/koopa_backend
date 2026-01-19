using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("CARRERA", Schema = "dbo")]
    public class Carrera
    {
        [Key]
        [Column("COD_CARRERA")]
        public int CodCarrera { get; set; }

        [Column("NOMBRE_CARRERA")]
        [MaxLength(150)]
        public string NombreCarrera { get; set; }

        [Column("COD_FACULTAD")]
        [MaxLength(150)]
        public string CodFacultad { get; set; }
    }
}