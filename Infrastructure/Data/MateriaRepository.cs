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

        public async Task<IEnumerable<MateriaMallaDto>> ObtenerDatosMallaAsync(int codCarrera)
        {
            // 1. Catálogo de Semestres
            var semestres = await _context.Semestres
                .AsNoTracking()
                .ToDictionaryAsync(k => k.CodSemestre, v => v.Nombre);

            // 2. Metadatos de Materias (Filtradas por Carrera)
            var materiasInfo = await (from mc in _context.MateriasCarrera
                                      join m in _context.Materias on mc.CodMateria equals m.CodMateria
                                      where mc.CodCarrera == codCarrera
                                      select new
                                      {
                                          m.CodMateria,
                                          m.NombreMateria,
                                          Nivel = mc.NivelCarrera
                                      }).AsNoTracking().ToListAsync();

            var idsMateriasCarrera = materiasInfo.Select(x => x.CodMateria).ToList();
            Console.WriteLine(idsMateriasCarrera);
            // 3. Obtener Requisitos (Optimización: Una sola consulta)
            // Se usa la entidad Requisito que definiste (tabla REQUISITOS)
            var requisitosRaw = await _context.Requisitos
                .AsNoTracking()
                .Where(r => idsMateriasCarrera.Contains(r.CodMateria))
                .ToListAsync();

            // Agrupamos en memoria por CodMateria para acceso rápido
            var requisitosMap = requisitosRaw
                .GroupBy(r => r.CodMateria)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 4. Estadísticas (Inscripciones)
            var estadisticasRaw = await _context.Inscripciones
                .AsNoTracking()
                .Where(i => idsMateriasCarrera.Contains((int)i.CodMateria))
                .GroupBy(i => new { i.CodMateria, i.CodSemestre })
                .Select(g => new
                {
                    CodMateria = (int)g.Key.CodMateria,
                    CodSemestre = (int)g.Key.CodSemestre,
                    TotalInscritos = g.Count(),
                    TotalReprobados = g.Count(x => (x.Promedio != null && x.Promedio < 6) || x.CodEstadoCurso == "REP")
                })
                .ToListAsync();

            var statsPorMateria = estadisticasRaw
                .GroupBy(x => x.CodMateria)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 5. Construcción del Resultado
            var resultado = new List<MateriaMallaDto>();

            foreach (var mat in materiasInfo)
            {
                // Datos de Estadísticas
                var statsDeEstaMateria = statsPorMateria.ContainsKey(mat.CodMateria)
                    ? statsPorMateria[mat.CodMateria]
                    : new();

                var historial = statsDeEstaMateria
                    .Select(s => new RendimientoMallaDto
                    {
                        Periodo = semestres.ContainsKey(s.CodSemestre) ? semestres[s.CodSemestre] : "Desconocido",
                        Inscripciones = s.TotalInscritos,
                        Reprobados = s.TotalReprobados
                    })
                    .OrderBy(h => h.Periodo)
                    .ToList();

                int totalInscritos = statsDeEstaMateria.Sum(x => x.TotalInscritos);
                int totalReprobados = statsDeEstaMateria.Sum(x => x.TotalReprobados);
                double porcentaje = totalInscritos > 0 ? (double)totalReprobados / totalInscritos : 0;
                string colorHex = porcentaje > 0.30 ? "#ef4444" : "#22c55e";

                // --- LOGICA DE REQUISITOS ---
                var misRequisitos = requisitosMap.ContainsKey(mat.CodMateria) 
                    ? requisitosMap[mat.CodMateria] 
                    : new List<Requisito>();

                // C. Prerrequisitos 
                // Usamos CodTipoRequisito para filtrar y CodMateriaRequisito para obtener el ID
                var listaPrerrequisitos = misRequisitos
                    .Where(r => r.CodTipoRequisito == "PR") 
                    .Select(r => r.CodMateriaRequisito.ToString()) 
                    .ToList();

                // D. Corequisitos
                var listaCorequisitos = misRequisitos
                    .Where(r => r.CodTipoRequisito == "CO" || r.CodTipoRequisito == "COR") 
                    .Select(r => r.CodMateriaRequisito.ToString())
                    .ToList();

                resultado.Add(new MateriaMallaDto
                {
                    Id = mat.CodMateria,
                    Codigo = $"MAT-{mat.CodMateria}",
                    Nombre = mat.NombreMateria,
                    Nivel = mat.Nivel ?? "SIN NIVEL",
                    Color = colorHex,
                    Rendimiento = historial,
                    Estado = "pendiente",
                    // Listas de IDs de materias requisito
                    PreRequisitos = listaPrerrequisitos,
                    CoRequisitos = listaCorequisitos,

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

            return resultado.OrderBy(x => x.Nivel).ThenBy(x => x.Nombre);
        }
    }
}