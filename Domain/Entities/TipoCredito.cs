using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("TIPO_CREDITO", Schema = "dbo")]
    public class TipoCredito
    {
        [Key]
        [Column("cod_tipo_credito")]
        public int CodTipoCredito { get; set; }

        [Column("tipo_credito")]
        [MaxLength(50)]
        public string Descripcion { get; set; }
    }
}