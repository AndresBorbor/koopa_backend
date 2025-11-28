using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("SEMESTRE", Schema = "dbo")]
    public class Semestre
    {
        [Key]
        [Column("cod_semestre")]
        public int CodSemestre { get; set; }

        [Column("anio")]
        public int? Anio { get; set; }

        [Column("termino")]
        [MaxLength(10)]
        public string Termino { get; set; }

        [Column("nombre")]
        [MaxLength(50)]
        public string Nombre { get; set; }

        [Column("fecha_inicio")]
        public DateTime? FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateTime? FechaFin { get; set; }

        [Column("fecha_inicio_clases")]
        public DateTime? FechaInicioClases { get; set; }

        [Column("fecha_fin_clases")]
        public DateTime? FechaFinClases { get; set; }

        [Column("fecha_inicio_plan")]
        public DateTime? FechaInicioPlan { get; set; }

        [Column("fecha_fin_plan")]
        public DateTime? FechaFinPlan { get; set; }
    }
}