using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    [Table("MATERIA_CARRERA", Schema = "dbo")]
    public class MateriaCarrera
    {
        // Nota: Las claves compuestas se definen usualmente en el DbContext, 
        // pero aquí mapeamos las columnas.
        
        [Column("COD_CARRERA")]
        public int CodCarrera { get; set; }

        [Column("COD_MATERIA")]
        public int CodMateria { get; set; }

        [Column("NIVEL_CARRERA")]
        [MaxLength(100)]
        public string NivelCarrera { get; set; }

        [Column("COD_TIPO_CREDITO")]
        public int? CodTipoCredito { get; set; }
    }
}