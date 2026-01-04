using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("PERIODO", Schema = "dbo")]
    public class Periodo
    {
        [Key]
        [Column("ANIO")]
        public int Anio { get; set; }

        [Column("TERMINO")]
        [MaxLength(100)]
        public string Termino { get; set; }

         [Column("NOMBRE_PERIODO")]
        [MaxLength(100)]
        public string NombrePeriodo { get; set; }

        [Column("FECHA_INICIO")]
        public DateTime FechaInicio { get; set; }

        [Column("FECHA_FIN")]
        public DateTime FechaFin { get; set; }

        [Column("FECHA_INICIO_CLASES")]
        public DateTime FechaInicioClases { get; set; }

        [Column("FECHA_FIN_CLASES")]
        public DateTime FechaFinClases { get; set; }

        [Column("FECHA_INICIO_PLAN")]
        public DateTime FechaInicioPlan { get; set; }

        [Column("FECHA_FIN_PLAN")]
        public DateTime FechaFinPlan { get; set; }
    }
}