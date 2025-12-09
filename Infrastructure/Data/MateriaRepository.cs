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
           
            // 1. Obtener datos de la VISTA SQL
            var datosVista = await _context.MallaStatsViews
                .AsNoTracking()
                .Where(x => x.CodCarrera == codCarrera)
                .OrderBy(x => x.NivelCarrera)
                .ThenBy(x => x.CodSemestre)
                .ToListAsync();
            if (!datosVista.Any()) return new List<MateriaMallaDto>();

            // 2. Obtener Requisitos para las flechas
            var idsMaterias = datosVista.Select(x => x.CodMateria).Distinct().ToList();
            
            var requisitosRaw = await _context.Requisitos
                .AsNoTracking()
                .Where(r => idsMaterias.Contains(r.CodMateria))
                .ToListAsync();

            var mapaRequisitos = requisitosRaw
                .GroupBy(r => r.CodMateria)
                .ToDictionary(g => g.Key, g => g.ToList());

            // 3. Agrupar y Mapear al DTO Nuevo
            var resultado = datosVista
                .GroupBy(x => new { x.CodMateria, x.NombreMateria, x.NivelCarrera })
                .Select(grupo => 
                {
                    // A. Llenar el Diccionario Stats
                    var diccionarioStats = new Dictionary<string, StatsMallaDto>();
                    
                    foreach(var fila in grupo)
                    {
                        double porcentaje = fila.InscritosActuales > 0 
                            ? (double)fila.ReprobadosSemestreAnterior / fila.InscritosActuales 
                            : 0;

                        diccionarioStats[fila.CodSemestre.ToString()] = new StatsMallaDto
                        {
                            Inscritos = fila.InscritosActuales,  // ✅ Ahora sí existe
                            Reprobados = fila.ReprobadosSemestreAnterior,
                            Habilitados = fila.HabilitadosParaTomarla,
                            ReprobadosPorcentaje = Math.Round(porcentaje, 2),
                            Descripcion = fila.NombreSemestre,
                            NotaPie = "DB2 View"
                        };
                    }
                    // B. Totales Generales
                    int totalInscritos = grupo.Sum(x => x.InscritosActuales);
                    int totalReprobados = grupo.Sum(x => x.ReprobadosSemestreAnterior);
                    double porcentajeGlobal = totalInscritos > 0 ? (double)totalReprobados / totalInscritos : 0;
                    string colorHex = porcentajeGlobal > 0.30 ? "#ef4444" : "#22c55e";

                    // C. Requisitos
                    var misReqs = mapaRequisitos.ContainsKey(grupo.Key.CodMateria) 
                        ? mapaRequisitos[grupo.Key.CodMateria] 
                        : new List<Requisito>();

                    // D. Retorno del DTO
                    return new MateriaMallaDto
                    {
                        Id = grupo.Key.CodMateria,
                        Codigo = $"{grupo.Key.CodMateria}",
                        Nombre = grupo.Key.NombreMateria,
                        Nivel = grupo.Key.NivelCarrera,
                        
                        // ✅ Propiedades que daban error antes, ahora funcionarán
                        CantidadEstudiantes = totalInscritos, 
                        Color = colorHex,
                        Estado = "disponible2",
                        
                        Stats = diccionarioStats, // ✅ Ahora acepta el diccionario
                                
                        PreRequisitos = misReqs.Where(r => r.CodTipoRequisito == "PR")
                                               .Select(r => r.CodMateriaRequisito.ToString()).ToList(),
                        CoRequisitos = misReqs.Where(r => r.CodTipoRequisito == "CO" || r.CodTipoRequisito == "COR")
                                              .Select(r => r.CodMateriaRequisito.ToString()).ToList(),
                        Rendimiento = diccionarioStats.Select(kv => new RendimientoMallaDto { 
                            Periodo = kv.Key, 
                            Inscripciones = kv.Value.Inscritos,
                            Reprobados = kv.Value.Reprobados
                        }).ToList()
                    };
                })
                .ToList();

            return resultado;
        }
    }
}