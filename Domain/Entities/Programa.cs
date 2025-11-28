using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("PROGRAMA", Schema = "dbo")]
    public class Programa
    {
        [Key]
        [Column("programa")]
        public int IdPrograma { get; set; }

        [Column("cod_semestre")]
        public int? CodSemestre { get; set; }

        [Column("total_ingreso_adm")]
        public int? TotalIngresoAdm { get; set; }

        [Column("total_ingreso_cambio")]
        public int? TotalIngresoCambio { get; set; }
    }
}