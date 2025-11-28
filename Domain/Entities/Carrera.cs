using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("CARRERA", Schema = "dbo")]
    public class Carrera
    {
        [Key]
        [Column("cod_carrera")]
        public int CodCarrera { get; set; }

        [Column("carrera")]
        [MaxLength(150)]
        public string NombreCarrera { get; set; }
    }
}