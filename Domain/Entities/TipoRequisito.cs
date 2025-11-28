using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("TIPO_REQUISITO", Schema = "dbo")]
    public class TipoRequisito
    {
        [Key]
        [Column("cod_tipo_requisito")]
        [MaxLength(10)]
        public string CodTipoRequisito { get; set; }

        [Column("tipo_requisito")]
        [MaxLength(50)]
        public string Descripcion { get; set; }
    }
}