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

       public async Task<IEnumerable<MateriaMallaDto>> ObtenerDatosMallaAsync(int codCarrera, string? codSemestre)
        {
            // 1. Obtener datos de la VISTA SQL
            var query = _context.MallaStatsViews
                .AsNoTracking()
                .Where(x => x.CodCarrera == codCarrera);


            if (!string.IsNullOrEmpty(codSemestre) && codSemestre != "all")
            {
                query = query.Where(x => x.CodSemestre == int.Parse(codSemestre));
            }

            var datosVista = await query
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
                    var diccionarioStats = new Dictionary<string, StatsMallaDto>();
                    
                    foreach(var fila in grupo)
                    {
                        double porcentaje = fila.InscritosActuales > 0 
                            ? (double)fila.ReprobadosSemestreAnterior / fila.InscritosActuales 
                            : 0;

                        // Usamos fila.CodSemestre.ToString() como Clave del diccionario
                        diccionarioStats[fila.CodSemestre.ToString()] = new StatsMallaDto
                        {
                            Inscritos = fila.InscritosActuales,
                            Reprobados = fila.ReprobadosSemestreAnterior,
                            Habilitados = fila.HabilitadosParaTomarla,
                            ReprobadosPorcentaje = Math.Round(porcentaje, 2),
                            Descripcion = fila.NombreSemestre,
                            NotaPie = "DB2 View"
                        };
                    }

                    // Totales
                    int totalInscritos = grupo.Sum(x => x.InscritosActuales);
                    int totalReprobados = grupo.Sum(x => x.ReprobadosSemestreAnterior);
                    double porcentajeGlobal = totalInscritos > 0 ? (double)totalReprobados / totalInscritos : 0;
                    string colorHex = porcentajeGlobal > 0.30 ? "#ef4444"       // Rojo
                                    : porcentajeGlobal > 0.15 ? "#eab308"       // Amarillo
                                    : "#22c55e";                                // Verde

                    string estado = porcentajeGlobal > 0.30 ? "Alto índice de reprobación"
                                : porcentajeGlobal > 0.15 ? "Índice de reprobación moderado"
                                : "Bajo índice de reprobación";
                    // Requisitos
                    var misReqs = mapaRequisitos.ContainsKey(grupo.Key.CodMateria) 
                        ? mapaRequisitos[grupo.Key.CodMateria] 
                        : new List<Requisito>();

                    return new MateriaMallaDto
                    {
                        Id = grupo.Key.CodMateria,
                        Codigo = $"{grupo.Key.CodMateria}",
                        Nombre = grupo.Key.NombreMateria,
                        Nivel = grupo.Key.NivelCarrera,
                        
                        CantidadEstudiantes = totalInscritos, 
                        Color = colorHex,
                        Estado = estado, 
                        Stats = diccionarioStats,
                                
                        PreRequisitos = misReqs.Where(r => r.CodTipoRequisito == "PR")
                                            .Select(r => r.CodMateriaRequisito.ToString()).ToList(),
                        CoRequisitos = misReqs.Where(r => r.CodTipoRequisito == "CO" || r.CodTipoRequisito == "COR")
                                            .Select(r => r.CodMateriaRequisito.ToString()).ToList(),
                        Rendimiento = new List<RendimientoMallaDto>()
                       
                    };
                })
                .ToList();

            return resultado;
        }
    }
}