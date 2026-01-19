using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("TIPO_REQUISITO", Schema = "dbo")]
    public class TipoRequisito
    {
        [Key]
        [Column("COD_TIPO_REQUISITO")]
        [MaxLength(10)]
        public string CodTipoRequisito { get; set; }

        [Column("NOMBRE_TIPO_REQUISITO")]
        [MaxLength(50)]
        public string NOMBRE_TIPO_REQUISITO { get; set; }
    }
}