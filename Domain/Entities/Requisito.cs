using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("REQUISITOS", Schema = "dbo")]
    public class Requisito
    {
        [Column("cod_materia")]
        public int CodMateria { get; set; }

        [Column("cod_materia_requisito")]
        public int CodMateriaRequisito { get; set; }

        [Column("cod_tipo_requisito")]
        [MaxLength(10)]
        public string CodTipoRequisito { get; set; }
    }
}