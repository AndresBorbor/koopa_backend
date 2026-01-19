using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("FACULTAD", Schema = "dbo")]
    public class Facultad
    {
        [Key]
        [Column("COD_FACULTAD")]
        public int CodFacultad { get; set; }

        [Column("SIGLAS")]
        [MaxLength(10)]
        public string Siglas { get; set; }

        [Column("NOMBRE_FACULTAD")]
        [MaxLength(100)]
        public string NombreFacultad { get; set; }
    }
}