using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("REQUISITOS", Schema = "dbo")]
    public class Requisito
    {
        [Column("COD_MATERIA")]
        public int CodMateria { get; set; }

        [Column("COD_MATERIA_REQUISITO")]
        public int CodMateriaRequisito { get; set; }

        [Column("COD_TIPO_REQUISITO")]
        [MaxLength(10)]
        public string CodTipoRequisito { get; set; }
    }
}