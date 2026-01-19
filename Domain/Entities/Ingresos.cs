using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("INGRESOS", Schema = "dbo")]
    public class Ingresos
    {
        [Key]
        [Column("COD_CARRERA")]
        public int CodCarrera { get; set; }

        [Column("ANIO")]
        public int Anio { get; set; }

        [Column("TERMINO")]
        public string Termino { get; set; }


        [Column("TOTAL_INGRESO_ADM")]
        public int TotalIngresoAdm { get; set; }

        [Column("TOTAL_INGRESO_CAMBIO")]
        public int TotalIngresoCambio { get; set; }
    }
}