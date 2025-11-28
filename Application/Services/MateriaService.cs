using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KoopaBackend.Application.DTOs; // Usamos el DTO
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;

namespace KoopaBackend.Application.Services
{
    public class MateriaService
    {
        private readonly IMateriaRepository _materiaRepo;
        private readonly IInscripcionesRepository _inscripcionRepo; // Necesitas crear este Repo
        private readonly ISemestreRepository _semestreRepo;       // Necesitas crear este Repo

        public MateriaService(
            IMateriaRepository materiaRepo,
            IInscripcionesRepository inscripcionRepo,
            ISemestreRepository semestreRepo)
        {
            _materiaRepo = materiaRepo;
            _inscripcionRepo = inscripcionRepo;
            _semestreRepo = semestreRepo;
        }

        public async Task<IEnumerable<MateriaMallaDto>> ObtenerDashboardStats()
        {
            // 1. Obtener Datos de la BD (Entidades)
            var materias = await _materiaRepo.GetAllAsync();
            var inscripciones = await _inscripcionRepo.GetAllAsync();
            var semestres = await _semestreRepo.GetAllAsync();

            var reporte = new List<MateriaMallaDto>();

            // 2. Procesar lógica en memoria (Mapping)
            foreach (var mat in materias)
            {
                // Filtramos inscripciones de esta materia
                var inscripcionesMateria = inscripciones
                    .Where(i => i.CodMateria == mat.CodMateria)
                    .ToList();

                // Si la materia no tiene historial, la saltamos (opcional)
                if (!inscripcionesMateria.Any()) continue;

                // A. Calcular Rendimiento por Semestre
                var rendimientoHistorico = inscripcionesMateria
                    .GroupBy(i => i.CodSemestre)
                    .Select(grupo => {
                        var sem = semestres.FirstOrDefault(s => s.CodSemestre == grupo.Key);
                        var nombreSemestre = sem != null ? sem.Nombre : "Desconocido"; // Ej: "I PAO 2023"
                        
                        // Lógica de reprobado (ajusta según tu DB, ej: estado 'REP' o promedio < 60)
                        int reprobadosCount = grupo.Count(x => x.CodEstadoCurso == "REP" || (x.Promedio != null && x.Promedio < 60));

                        return new RendimientoMallaDto
                        {
                            Periodo = nombreSemestre,
                            Inscripciones = grupo.Count(),
                            Reprobados = reprobadosCount
                        };
                    })
                    .OrderBy(r => r.Periodo)
                    .ToList();

                // B. Calcular Estadísticas Generales (Stats)
                var totalInscritos = inscripcionesMateria.Count();
                var totalReprobados = inscripcionesMateria.Count(x => x.CodEstadoCurso == "REP" || (x.Promedio != null && x.Promedio < 60));
                var porcentajeReprobacion = totalInscritos > 0 ? (double)totalReprobados / totalInscritos : 0;

                // Definir color basado en riesgo
                string colorEstado = porcentajeReprobacion > 0.3 ? "#ef4444" : "#22c55e"; // Rojo si > 30% reprueba

                // 3. Crear el DTO final
                var dto = new MateriaMallaDto
                {
                    Id = mat.CodMateria,
                    Codigo = $"MAT-{mat.CodMateria}", // Generar código visual
                    Nombre = mat.NombreMateria,
                    Nivel = "NIVEL GENERAL", // Esto vendría de MateriaCarrera si lo tuvieras cargado
                    Color = colorEstado,
                    
                    Rendimiento = rendimientoHistorico,
                    
                    Stats = new StatsMallaDto
                    {
                        Reprobados = totalReprobados,
                        ReprobadosPorcentaje = Math.Round(porcentajeReprobacion, 2),
                        AprobaronRequisitos = totalInscritos - totalReprobados, // Lógica simple
                        Habilitados = totalInscritos + 15, // Simulación de proyección
                        Descripcion = $"Análisis histórico de {mat.NombreMateria}.",
                        NotaPie = "Datos de DB2"
                    }
                };

                reporte.Add(dto);
            }

            return reporte;
        }
    }
}