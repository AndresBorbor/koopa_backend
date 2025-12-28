using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    // Match DB2 table USUARIO in schema DBO and enforce DB column lengths to avoid truncation.
    [Table("USUARIO", Schema = "DBO")]
    public class Usuario
    {
        [Key]
        [Column("NOMBRE_USUARIO")]
        [MaxLength(50)]
        public string NombreUsuario { get; set; }

        [Column("PASSWORD")]
        [MaxLength(50)]
        public string Password { get; set; }

        [Column("COD_CARRERA")]
        public int? CodCarrera { get; set; }
    }
}

