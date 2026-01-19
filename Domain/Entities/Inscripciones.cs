using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities // O KoopaBackend.Domain.Entities
{
    [Table("INSCRIPCIONES", Schema = "dbo")]
    public class Inscripciones 
    {
        [Key]
        [Column("COD_INSCRIPCION")]
        public int CodInscripcion { get; set; }

        [Column("ANIO")]
        public int Anio { get; set; }

        [Column("TERMINO")]
        public string Termino { get; set; }

        [Column("COD_PARALELO")]
        public int CodParalelo { get; set; }

        [Column("COD_CURSO")]
        public int CodCurso { get; set; }

        [Column("COD_MATERIA")]
        public int CodMateria { get; set; }

        [Column("COD_ESTUDIANTE")]
        public long CodEstudiante { get; set; }

        [Column("COD_ESTADO_CURSO")]
        [MaxLength(5)]
        public string CodEstadoCurso { get; set; }

        [Column("PROMEDIO")]
        public decimal? Promedio { get; set; }
    }
}