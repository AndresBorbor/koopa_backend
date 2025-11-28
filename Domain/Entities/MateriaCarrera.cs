using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("MATERIA_CARRERA", Schema = "dbo")]
    public class MateriaCarrera
    {
        // Nota: Las claves compuestas se definen usualmente en el DbContext, 
        // pero aquí mapeamos las columnas.
        
        [Column("cod_carrera")]
        public int CodCarrera { get; set; }

        [Column("cod_materia")]
        public int CodMateria { get; set; }

        [Column("nivel_carrera")]
        [MaxLength(20)]
        public string NivelCarrera { get; set; }

        [Column("cod_tipo_credito")]
        public int? CodTipoCredito { get; set; }
    }
}