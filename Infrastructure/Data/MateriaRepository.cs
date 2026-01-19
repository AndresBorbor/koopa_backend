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


        public async Task<IEnumerable<MateriaMallaDto>> ObtenerDatosMallaAsync(int codCarrera, int? anio, string? termino)
{
    try
    {
        // 1. Query base
        var queryMetricasMateria = _context.VwMetricasMaterias
            .AsNoTracking()
            .Where(x => x.CodCarrera == codCarrera);

            if (anio.HasValue)
            {
                queryMetricasMateria = queryMetricasMateria
                    .Where(x => x.Anio == anio.Value);
            }

            if (!string.IsNullOrEmpty(termino))
            {
                queryMetricasMateria = queryMetricasMateria
                    .Where(x => x.Termino.ToLower() == termino.ToLower());
            }



        // 2. Proyección segura a un DTO intermedio para evitar InvalidCast
        var listaMetricas = await queryMetricasMateria
            .Select(x => new
            {
                x.CodMateria,
                NombreMateria = x.NombreMateria ?? "N/A",
                x.CodCarrera,
                NivelCarrera = x.NivelCarrera ?? "N/A",
                CantidadEstudiantes = (int?)x.CantidadEstudiantes,
                CantidadInscripciones = (int?)x.CantidadInscripciones,
                CantidadReprobados = (int?)x.CantidadReprobados,
                PromedioMateria = (decimal?)x.PromedioMateria, 
                NombrePeriodo = x.NombrePeriodo ?? "N/A",   // <- Aquí está el fix
                x.Anio,
                x.Termino
            })
            .ToListAsync();


        // if (listaMetricas == null || !listaMetricas.Any())
        // {
        //     Console.WriteLine("No se encontraron registros para la consulta.");
        //     return new List<MateriaMallaDto>();
        // }

        // 3. Construir MateriaMallaDto en memoria
        var materias = listaMetricas
            .GroupBy(m => m.CodMateria)
            .Select(g =>
            {
                var materia = g.First();

                // Estado y color
                double? reprobPorcentaje = materia.CantidadEstudiantes == 0
                    ? 0
                    : (double)materia.CantidadReprobados / materia.CantidadEstudiantes;

                string estado = materia.CantidadEstudiantes == 0
                    ? "Sin Inscripciones"
                    : reprobPorcentaje >= 0.7 ? "Alto índice de Reprobación"
                    : reprobPorcentaje >= 0.4 ? "Moderado índice de Reprobación"
                    : "Bajo índice de Reprobación";

                string color = materia.CantidadEstudiantes == 0
                    ? "#808080"
                    : reprobPorcentaje >= 0.7 ? "#FF0000"
                    : reprobPorcentaje >= 0.4 ? "#FFFF00"
                    : "#008000";

                // Stats por periodo
                var stats = g.GroupBy(x => $"{x.Anio}-{x.Termino}")
                             .ToDictionary(
                                s => s.Key,
                                s => new StatsMallaDto
                                {
                                    Inscritos = s.Sum(x => x.CantidadInscripciones ?? 0),
                                    Reprobados = s.Sum(x => x.CantidadReprobados ?? 0),
                                    ReprobadosPorcentaje = s.Sum(x => x.CantidadInscripciones ?? 0) == 0
                                        ? 0
                                        : (double)(s.Sum(x => x.CantidadReprobados ?? 0)) / s.Sum(x => x.CantidadInscripciones ?? 0),
                                    Habilitados = 0,
                                    Descripcion = s.Key,
                                    NotaPie = "Nota al pie agregada"
                                });

                var cantidadInscripciones = g.Sum(x => x.CantidadInscripciones ?? 0);

                return new MateriaMallaDto
                {
                    Id = materia.CodMateria,
                    Codigo = materia.CodMateria.ToString(),
                    Nombre = materia.NombreMateria,
                    Nivel = materia.NivelCarrera,
                    CantidadEstudiantes = cantidadInscripciones,
                    Estado = estado,
                    Color = color,
                    Stats = stats,
                    Rendimiento = g.Select(x => new RendimientoMallaDto
                    {
                        Periodo = $"{x.Anio}-{x.Termino}",
                        Inscripciones = x.CantidadInscripciones ?? 0,
                        Reprobados = x.CantidadReprobados ?? 0
                    }).ToList(),
                    PreRequisitos = _context.Requisitos
                        .Where(r => r.CodMateria == materia.CodMateria && r.CodTipoRequisito == "PR")
                        .Select(r => r.CodMateriaRequisito.ToString())
                        .ToList(),
                    CoRequisitos = _context.Requisitos
                        .Where(r => r.CodMateria == materia.CodMateria && r.CodTipoRequisito == "CO")
                        .Select(r => r.CodMateriaRequisito.ToString())
                        .ToList()
                };
            })
            .ToList();

        return materias;
    }
    catch (Exception ex)
    {
        // Log detallado
        Console.WriteLine("==================================");
        Console.WriteLine("ERROR al ejecutar ObtenerDatosMallaAsync");
        Console.WriteLine(ex.GetType().FullName);
        Console.WriteLine(ex.Message);
        Console.WriteLine(ex.StackTrace);
        Console.WriteLine("==================================");

        // Opcional: relanzar, o retornar lista vacía
        return new List<MateriaMallaDto>();
    }
}






    }
}