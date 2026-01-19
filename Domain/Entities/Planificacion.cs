using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("PLANIFICACION", Schema = "dbo")]
    public class Planificacion
    {
        [Column("COD_CURSO")]
        public int CodCurso { get; set; }

        [Column("ANIO")]
        public int Anio { get; set; }

        [Column("TERMINO")]
        public int Termino { get; set; }

        [Column("COD_MATERIA")]
        public int CodMateria { get; set; }

        [Column("COD_PARALELO")]
        public int CodParalelo { get; set; }

        [Column("CUPO_PLANIFICADO")]
        public int CupoPlanificado { get; set; }

        [Column("PARALELO_CANCELADO")]
        [MaxLength(1)]
        public string ParaleloCancelado { get; set; }

        [Column("APROBADO")]
        [MaxLength(1)]
        public string Aprobado { get; set; }

        [Column("INSCRITOS")]
        public int Inscritos { get; set; }
    }
}