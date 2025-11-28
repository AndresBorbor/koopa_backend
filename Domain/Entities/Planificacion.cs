using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("PLANIFICACION", Schema = "dbo")]
    public class Planificacion
    {
        [Key]
        [Column("cod_planificacion")]
        public int CodPlanificacion { get; set; }

        [Column("cod_curso")]
        public int? CodCurso { get; set; }

        [Column("cod_materia")]
        public int? CodMateria { get; set; }

        [Column("cod_semestre")]
        public int? CodSemestre { get; set; }

        [Column("cod_paralelo")]
        public int? CodParalelo { get; set; }

        [Column("cupo_planificado")]
        public int? CupoPlanificado { get; set; }

        [Column("paralelo_cancelado")]
        [MaxLength(1)]
        public string ParaleloCancelado { get; set; }

        [Column("aprobado")]
        [MaxLength(1)]
        public string Aprobado { get; set; }
    }
}