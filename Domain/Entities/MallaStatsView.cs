using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KoopaBackend.Domain.Entities
{
    public class MallaStatsView
    {
        public int CodCarrera { get; set; }
        public int CodMateria { get; set; }
        public string NombreMateria { get; set; }
        public string NivelCarrera { get; set; }
        
        public int CodSemestre { get; set; }
        public string NombreSemestre { get; set; }
        
        // Estas columnas vienen calculadas desde SQL
        public int InscritosActuales { get; set; }
        public int ReprobadosSemestreAnterior { get; set; }
        public int HabilitadosParaTomarla { get; set; }
    }
}