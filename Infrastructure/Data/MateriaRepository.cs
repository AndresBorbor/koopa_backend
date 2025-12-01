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

        // CAMBIO 1: Agregamos el parámetro codCarrera
        public async Task<IEnumerable<MateriaMallaDto>> ObtenerDatosMallaAsync(int codCarrera)
        {
            // 1. Catálogo de Semestres (Igual que antes)
            var semestres = await _context.Semestres
                .AsNoTracking()
                .ToDictionaryAsync(k => k.CodSemestre, v => v.Nombre);

            // 2. Metadatos de Materias CON NIVEL (EL CAMBIO IMPORTANTE)
            // Asumimos que tienes un DbSet<MateriaCarrera> llamado MateriasCarrera.
            // Si no lo tienes mapeado directo, usaremos un Join manual de LINQ.
            
            var materiasInfo = await (from mc in _context.MateriasCarrera
                                      join m in _context.Materias on mc.CodMateria equals m.CodMateria
                                      where mc.CodCarrera == codCarrera // Filtramos por la carrera solicitada
                                      select new 
                                      {
                                          m.CodMateria,
                                          m.NombreMateria,
                                          // Aquí obtenemos el nivel real de la tabla MATERIA_CARRERA
                                          Nivel = mc.NivelCarrera 
                                      }).AsNoTracking().ToListAsync();

            // Extraemos solo los IDs de las materias de esta carrera para optimizar la consulta de inscripciones
            var idsMateriasCarrera = materiasInfo.Select(x => x.CodMateria).ToList();

            // 3. Aggregation (Estadísticas)
            // Optimizamos: .Where(...) para no traer inscripciones de materias que no son de esta carrera
            var estadisticasRaw = await _context.Inscripciones
                .AsNoTracking()
                .Where(i => idsMateriasCarrera.Contains((int)i.CodMateria)) 
                .GroupBy(i => new { i.CodMateria, i.CodSemestre })
                .Select(g => new 
                {
                    CodMateria = (int)g.Key.CodMateria,
                    CodSemestre = (int)g.Key.CodSemestre,
                    TotalInscritos = g.Count(),
                    // Lógica de reprobados mantenida
                    TotalReprobados = g.Count(x => (x.Promedio != null && x.Promedio < 60) || x.CodEstadoCurso == "REP")
                })
                .ToListAsync();

            // 4. Procesamiento en Memoria
            var resultado = new List<MateriaMallaDto>();

            var statsPorMateria = estadisticasRaw
                .GroupBy(x => x.CodMateria)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Iteramos sobre las materias FILTRADAS por carrera y con su NIVEL real
            foreach (var mat in materiasInfo)
            {
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
                    Codigo = $"MAT-{mat.CodMateria}", // Ajuste estético opcional
                    Nombre = mat.NombreMateria,
                    
                    // CAMBIO 3: Asignamos el nivel real traído de la base de datos
                    Nivel = mat.Nivel ?? "SIN NIVEL", 
                    
                    Color = colorHex,
                    Rendimiento = historial,
                    Stats = new StatsMallaDto
                    {
                        Reprobados = totalReprobados,
                        ReprobadosPorcentaje = Math.Round(porcentaje, 2),
                        AprobaronRequisitos = totalInscritos - totalReprobados,
                        Habilitados = totalInscritos + 10, // Tu lógica de negocio original
                        Descripcion = $"Datos consolidados para {mat.NombreMateria}",
                        NotaPie = "Fuente: DB2 Académico"
                    }
                });
            }

            // Opcional: Ordenar por Nivel para que el JSON salga ordenado (ej: Nivel 1, Nivel 2...)
            return resultado.OrderBy(x => x.Nivel).ThenBy(x => x.Nombre);
        }
    }
}