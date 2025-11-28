using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data; // Tu DbContext
using KoopaBackend.Application.DTOs;    // Tu DTO



namespace KoopaBackend.Infrastructure.Repositories
{
    public class MateriaRepository : IMateriaRepository
    {
        private readonly KoopaDbContext _context;

        public MateriaRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Materia>> GetAllAsync()
        {
            return await _context.Materias.ToListAsync();
        }

        public async Task<IEnumerable<MateriaMallaDto>> ObtenerDatosMallaAsync()
        {
            // 1. Traer datos crudos (Raw Data) haciendo JOIN en memoria o DB.
            // Traemos todo para procesarlo en memoria (Client Evaluation) 
            // porque la agrupación anidada compleja a veces falla en EF Core directo a SQL.
            
            var materias = await _context.Materias.ToListAsync();
            var inscripciones = await _context.Inscripciones.ToListAsync();
            var semestres = await _context.Semestres.ToListAsync();

            var resultado = new List<MateriaMallaDto>();

            foreach (var mat in materias)
            {
                // Filtramos inscripciones de esta materia
                var inscripcionesDeMateria = inscripciones.Where(i => i.CodMateria == mat.CodMateria).ToList();

                // Si no tiene datos, decidimos si agregarla vacía o saltarla. 
                // La agregamos vacía para que salga en la malla.
                
                // A. Calcular Historial (Rendimiento)
                var historial = inscripcionesDeMateria
                    .GroupBy(i => i.CodSemestre)
                    .Select(grupo => {
                        var sem = semestres.FirstOrDefault(s => s.CodSemestre == grupo.Key);
                        var nombrePeriodo = sem != null ? sem.Nombre : "Desconocido"; // Ej: "I PAO 2023"
                        
                        // Lógica de Reprobado: (Promedio < 60 o Estado 'REP')
                        int reprobados = grupo.Count(x => (x.Promedio != null && x.Promedio < 60) || x.CodEstadoCurso == "REP");

                        return new RendimientoMallaDto
                        {
                            Periodo = nombrePeriodo,
                            Inscripciones = grupo.Count(),
                            Reprobados = reprobados
                        };
                    })
                    .OrderBy(h => h.Periodo) // Ordenar si es posible
                    .ToList();

                // B. Calcular Stats Generales
                int totalInscritos = inscripcionesDeMateria.Count;
                int totalReprobados = inscripcionesDeMateria.Count(x => (x.Promedio != null && x.Promedio < 60) || x.CodEstadoCurso == "REP");
                double porcentaje = totalInscritos > 0 ? (double)totalReprobados / totalInscritos : 0;

                // C. Determinar Color
                // Verde (#22c55e) si reprobación < 30%, Rojo (#ef4444) si es mayor.
                string colorHex = porcentaje > 0.30 ? "#ef4444" : "#22c55e"; 

                // D. Armar el objeto final
                var dto = new MateriaMallaDto
                {
                    Id = mat.CodMateria,
                    Codigo = $"MATG-{mat.CodMateria}", // Código simulado
                    Nombre = mat.NombreMateria,
                    Nivel = "NIVEL GENÉRICO", // Esto vendría de MateriaCarrera si lo tuviéramos
                    Color = colorHex,
                    
                    Rendimiento = historial,
                    
                    Stats = new StatsMallaDto
                    {
                        Reprobados = totalReprobados,
                        ReprobadosPorcentaje = Math.Round(porcentaje, 2),
                        
                        // Estos datos son simulados porque requieren lógica de pre-requisitos compleja
                        AprobaronRequisitos = totalInscritos - totalReprobados, 
                        Habilitados = totalInscritos + 10, 
                        
                        Descripcion = $"Datos consolidados para {mat.NombreMateria}",
                        NotaPie = "Fuente: DB2 Académico"
                    }
                };

                resultado.Add(dto);
            }

            return resultado;
        }
    }
}