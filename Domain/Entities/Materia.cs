using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("MATERIA", Schema = "dbo")]
    public class Materia
    {
        [Key]
        [Column("COD_MATERIA")]
        public int CodMateria { get; set; }

        [Column("NOMBRE_MATERIA")]
        [MaxLength(100)]
        public string NombreMateria { get; set; }
    
    }
}