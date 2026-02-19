using KoopaBackend.Application.DTOs;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KoopaBackend.Application.Services
{
    public class MateriaService
    {
        private readonly IMateriaRepository _repository;

        public MateriaService(IMateriaRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MateriaMallaDto>> ObtenerMallaAsync(
            int codCarrera,
            int? anio,
            string? termino)
        {

            var metricas = await _repository
                .ObtenerMetricasMateriasAsync(codCarrera, anio, termino);

          
            var requisitos = await _repository
                .ObtenerRequisitosAsync();


            var materias = metricas
                .GroupBy(m => m.CodMateria)
                .Select(g =>
                {
                    var materia = g.First();

                    int inscritos = g.Sum(x => x.CantidadInscripciones);
                    int reprobados = g.Sum(x => x.CantidadReprobados);

                    double porcentajeReprob = inscritos == 0
                        ? 0
                        : (double)reprobados / inscritos;

                    string estado =
                        inscritos == 0 ? "Sin Inscripciones" :
                        porcentajeReprob >= 0.7 ? "Alto índice de Reprobación" :
                        porcentajeReprob >= 0.4 ? "Moderado índice de Reprobación" :
                        "Bajo índice de Reprobación";

                    string color =
                        inscritos == 0 ? "#808080" :
                        porcentajeReprob >= 0.7 ? "#FF0000" :
                        porcentajeReprob >= 0.4 ? "#FFFF00" :
                        "#008000";

                    var stats = g
                        .GroupBy(x => $"{x.Anio}-{x.Termino}")
                        .ToDictionary(
                            s => s.Key,
                            s => new StatsMallaDto
                            {
                                Inscritos = s.Sum(x => x.CantidadInscripciones),
                                Reprobados = s.Sum(x => x.CantidadReprobados),
                                ReprobadosPorcentaje =
                                    s.Sum(x => x.CantidadInscripciones) == 0
                                        ? 0
                                        : (double)s.Sum(x => x.CantidadReprobados)
                                          / s.Sum(x => x.CantidadInscripciones),
                                Habilitados = 0,
                                Descripcion = "Descripción agregada",
                                NotaPie = "Nota al pie agregada"
                            });
                    return new MateriaMallaDto
                    {
                        Id = materia.CodMateria,
                        Codigo = materia.CodMateria.ToString(),
                        Nombre = materia.NombreMateria ?? "N/A",
                        CodigoMateria = materia.CodigoMateria ?? "N/A",
                        Nivel = materia.NivelCarrera ?? "N/A",
                        CantidadEstudiantes = inscritos,
                        Estado = estado,
                        Color = color,
                        Stats = stats,
                        Rendimiento = g.Select(x => new RendimientoMallaDto
                        {
                            Periodo = $"{x.Anio}-{x.Termino}",
                            Inscripciones = x.CantidadInscripciones,
                            Reprobados = x.CantidadReprobados
                        }).ToList(),
                        PreRequisitos = requisitos
                            .Where(r => r.CodMateria == materia.CodMateria && r.CodTipoRequisito == "PR")
                            .Select(r => r.CodMateriaRequisito.ToString())
                            .ToList(),
                        CoRequisitos = requisitos
                            .Where(r => r.CodMateria == materia.CodMateria && r.CodTipoRequisito == "CO")
                            .Select(r => r.CodMateriaRequisito.ToString())
                            .ToList()
                    };
                })
                .ToList();

            return materias;
        }
    }
}
