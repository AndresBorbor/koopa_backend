using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities // O KoopaBackend.Domain.Entities
{
    [Table("INSCRIPCIONES", Schema = "dbo")]
    public class Inscripciones 
    {
        [Key]
        [Column("cod_inscripcion")]
        public int CodInscripcion { get; set; }

        [Column("cod_semestre")]
        public int? CodSemestre { get; set; }

        [Column("cod_paralelo")]
        public int? CodParalelo { get; set; }

        [Column("cod_curso")]
        public int? CodCurso { get; set; }

        [Column("cod_materia")]
        public int? CodMateria { get; set; }

        [Column("cod_estudiante")]
        public long? CodEstudiante { get; set; }

        [Column("cod_estado_curso")]
        [MaxLength(5)]
        public string CodEstadoCurso { get; set; }

        [Column("promedio")]
        public decimal? Promedio { get; set; }
    }
}