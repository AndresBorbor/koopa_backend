using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;
using KoopaBackend.Application.DTOs;

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
            return await _context.Materias.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<MateriaMallaDto>> ObtenerDatosMallaAsync()
        {
            // 1. Catálogo de Semestres
            // Al ser 'int' estricto, no necesitamos trucos aquí.
            var semestres = await _context.Semestres
                                          .AsNoTracking()
                                          .ToDictionaryAsync(k => k.CodSemestre, v => v.Nombre);

            // 2. Metadatos de Materias
            var materias = await _context.Materias
                                         .AsNoTracking()
                                         .Select(m => new { m.CodMateria, m.NombreMateria })
                                         .ToListAsync();

            // 3. Aggregation (La parte crítica)
            var estadisticasRaw = await _context.Inscripciones
                .AsNoTracking()
                .GroupBy(i => new { i.CodMateria, i.CodSemestre })
                .Select(g => new 
                {
                    // FIX: Casteamos explícitamente a (int). 
                    // Esto le dice al compilador: "Confía en mí, esto NO es nullable".
                    CodMateria = (int)g.Key.CodMateria,
                    CodSemestre = (int)g.Key.CodSemestre,
                    
                    TotalInscritos = g.Count(),
                    // Asumimos que Promedio puede ser null (double?), pero la comparación < 60 funciona igual
                    TotalReprobados = g.Count(x => (x.Promedio != null && x.Promedio < 60) || x.CodEstadoCurso == "REP")
                })
                .ToListAsync();

            // 4. Procesamiento en Memoria
            var resultado = new List<MateriaMallaDto>();

            // Ahora el Key del diccionario será estrictamente 'int'
            var statsPorMateria = estadisticasRaw
                                    .GroupBy(x => x.CodMateria)
                                    .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var mat in materias)
            {
                // Como mat.CodMateria es int y el diccionario es <int, List>, esto ya no debe fallar
                var statsDeEstaMateria = statsPorMateria.ContainsKey(mat.CodMateria) 
                    ? statsPorMateria[mat.CodMateria] 
                    : new();

                // A. Historial
                var historial = statsDeEstaMateria
                    .Select(s => new RendimientoMallaDto
                    {
                        Periodo = semestres.ContainsKey(s.CodSemestre) ? semestres[s.CodSemestre] : "Desconocido",
                        Inscripciones = s.TotalInscritos,
                        Reprobados = s.TotalReprobados
                    })
                    .OrderBy(h => h.Periodo)
                    .ToList();

                // B. Totales
                int totalInscritos = statsDeEstaMateria.Sum(x => x.TotalInscritos);
                int totalReprobados = statsDeEstaMateria.Sum(x => x.TotalReprobados);
                double porcentaje = totalInscritos > 0 ? (double)totalReprobados / totalInscritos : 0;

                string colorHex = porcentaje > 0.30 ? "#ef4444" : "#22c55e";

                resultado.Add(new MateriaMallaDto
                {
                    Id = mat.CodMateria,
                    Codigo = $"MATG-{mat.CodMateria}",
                    Nombre = mat.NombreMateria,
                    Nivel = "NIVEL GENÉRICO",
                    Color = colorHex,
                    Rendimiento = historial,
                    Stats = new StatsMallaDto
                    {
                        Reprobados = totalReprobados,
                        ReprobadosPorcentaje = Math.Round(porcentaje, 2),
                        AprobaronRequisitos = totalInscritos - totalReprobados,
                        Habilitados = totalInscritos + 10,
                        Descripcion = $"Datos consolidados para {mat.NombreMateria}",
                        NotaPie = "Fuente: DB2 Académico"
                    }
                });
            }

            return resultado;
        }
    }
}