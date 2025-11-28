using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("FACULTAD", Schema = "dbo")]
    public class Facultad
    {
        [Key]
        [Column("cod_facultad")]
        public int CodFacultad { get; set; }

        [Column("siglas")]
        [MaxLength(10)]
        public string Siglas { get; set; }

        [Column("facultad")]
        [MaxLength(100)]
        public string NombreFacultad { get; set; }
    }
}