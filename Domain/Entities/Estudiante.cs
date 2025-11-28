using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("ESTUDIANTE", Schema = "dbo")]
    public class Estudiante
    {
        [Key]
        [Column("cod_estudiante")]
        public long CodEstudiante { get; set; }

        [Column("cod_carrera")]
        public int? CodCarrera { get; set; }
    }
}