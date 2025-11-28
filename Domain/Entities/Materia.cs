using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("MATERIA", Schema = "dbo")]
    public class Materia
    {
        [Key]
        [Column("cod_materia")]
        public int CodMateria { get; set; }

        [Column("materia")]
        [Required]
        [MaxLength(200)]
        public string NombreMateria { get; set; }

        [Column("cod_tipo_credito")]
        public int? CodTipoCredito { get; set; }
        
        // Propiedades de navegación (Opcionales, ayudan en el Repo)
        // public virtual ICollection<Inscripcion> Inscripciones { get; set; }
    }
}