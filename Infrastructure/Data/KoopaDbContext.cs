using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;

namespace KoopaBackend.Infrastructure.Data
{
    public class KoopaDbContext : DbContext
    {
        public KoopaDbContext(DbContextOptions<KoopaDbContext> options) : base(options) { }

        // =========================================================================
        // 1. TABLAS MAESTRAS (Catálogos)
        // =========================================================================
        public DbSet<Facultad> Facultades { get; set; }
        public DbSet<Carrera> Carreras { get; set; }
        public DbSet<TipoCredito> TiposCredito { get; set; }
        public DbSet<TipoRequisito> TiposRequisito { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Graduados> Graduados { get; set; }
        public DbSet<Ingresos> Ingresos { get; set; }
        public DbSet<Periodo> Periodos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        // =========================================================================
        // 2. TABLAS PUENTE (Relaciones N a M)
        // =========================================================================
        public DbSet<MateriaCarrera> MateriasCarrera { get; set; }
        public DbSet<Requisito> Requisitos { get; set; }

        // =========================================================================
        // 3. TABLAS TRANSACCIONALES (Hechos)
        // =========================================================================
        public DbSet<Planificacion> Planificaciones { get; set; }
        public DbSet<Inscripciones> Inscripciones { get; set; } // Tu clase se llama 'Inscripciones'

        public DbSet<MallaStatsView> MallaStatsViews { get; set; }
        public DbSet<VwMetricasMateriaPeriodo> VwMetricasMateriaPeriodos{ get; set; }

        // 👇 ESTE ES EL BLOQUE QUE TE FALTA O ESTÁ INCOMPLETO
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. SOLUCIÓN PARA MATERIA_CARRERA
            // Le decimos: "Tu clave primaria es la combinación de Carrera + Materia"
            modelBuilder.Entity<MateriaCarrera>()
                .HasKey(mc => new { mc.CodCarrera, mc.CodMateria });

            // 2. SOLUCIÓN PARA REQUISITOS (Para evitar el siguiente error)
            // Le decimos: "Tu clave es Materia + Requisito + Tipo"
            modelBuilder.Entity<Requisito>()
                .HasKey(r => new { r.CodMateria, r.CodMateriaRequisito, r.CodTipoRequisito });

            // Mapear vista como Keyless
            modelBuilder.Entity<VwMetricasMateriaPeriodo>()
                .HasNoKey()
                .ToView("VW_METRICAS_MATERIA_PERIODO", "dbo");

            modelBuilder.Entity<MallaStatsView>()
                .HasNoKey()
                .ToView("VW_MATERIAS_MALLA", "dbo");

            modelBuilder.Entity<Planificacion>()
                .HasNoKey();

            modelBuilder.Entity<VwMetricasMateriaPeriodo>(entity =>
            {
                entity.ToView("VW_METRICAS_MATERIA_PERIODO", "dbo");
                entity.HasNoKey();

                entity.Property(e => e.Anio).HasColumnName("ANIO");
                entity.Property(e => e.Termino).HasColumnName("TERMINO");
                entity.Property(e => e.CodCarrera).HasColumnName("COD_CARRERA");
                entity.Property(e => e.NombreCarrera).HasColumnName("NOMBRE_CARRERA");
                entity.Property(e => e.CodMateria).HasColumnName("COD_MATERIA");
                entity.Property(e => e.NombreMateria).HasColumnName("NOMBRE_MATERIA");
                entity.Property(e => e.Inscritos).HasColumnName("INSCRITOS");
                entity.Property(e => e.Reprobados).HasColumnName("REPROBADOS");
                entity.Property(e => e.Aprobados).HasColumnName("APROBADOS");
                entity.Property(e => e.PromedioMateria).HasColumnName("PROMEDIO_MATERIA");   

            });
            
            // modelBuilder.Entity<MallaStatsView>(entity =>
            //     {
            //         entity.ToView("VW_MATERIAS_MALLA", "dbo");
            //         entity.HasNoKey();

            //         entity.Property(e => e.CodCarrera).HasColumnName("COD_CARRERA");
            //         entity.Property(e => e.CodMateria).HasColumnName("COD_MATERIA");
            //         entity.Property(e => e.NombreMateria).HasColumnName("NOMBRE_MATERIA");
            //         entity.Property(e => e.NivelCarrera).HasColumnName("NIVEL_CARRERA");
            //         entity.Property(e => e.CodSemestre).HasColumnName("COD_SEMESTRE");
            //         entity.Property(e => e.NombreSemestre).HasColumnName("NOMBRE_SEMESTRE");
            //         entity.Property(e => e.InscritosActuales).HasColumnName("INSCRITOS_ACTUALES");
            //         entity.Property(e => e.ReprobadosSemestreAnterior).HasColumnName("REPROBADOS_SEMESTRE_ANTERIOR");
            //         entity.Property(e => e.HabilitadosParaTomarla).HasColumnName("HABILITADOS_PARA_TOMARLA");
            //     });

            

        }


    }
}