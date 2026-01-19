using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("TIPO_CREDITO", Schema = "dbo")]
    public class TipoCredito
    {
        [Key]
        [Column("COD_TIPO_CREDITO")]
        public int CodTipoCredito { get; set; }

        [Column("NOMBRE_TIPO_CREDITO")]
        [MaxLength(50)]
        public string NOMBRE_TIPO_CREDITO { get; set; }
    }
}