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
        public DbSet<VW_MetricasCarreraAnio> VwMetricasCarreraAnios{ get; set; }
        public DbSet<VW_MetricasCarreraPeriodo> VwMetricasCarreraPeriodos{ get; set; }
        public DbSet<VW_MetricasMateria> VwMetricasMaterias{ get; set; }

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
            modelBuilder.Entity<Planificacion>()
                .HasNoKey();

            modelBuilder.Entity<VW_MetricasCarreraAnio>(entity =>
            {
                entity.ToView("VW_METRICAS_CARRERA_ANIO", "dbo");
                entity.HasNoKey();

                
                entity.Property(e => e.Anio).HasColumnName("ANIO");
                entity.Property(e => e.CodCarrera).HasColumnName("COD_CARRERA");
                entity.Property(e => e.NombreCarrera).HasColumnName("NOMBRE_CARRERA");
                entity.Property(e => e.CantidadEstudiantes).HasColumnName("CANTIDAD_ESTUDIANTES");
                entity.Property(e => e.CantidadInscripciones).HasColumnName("CANTIDAD_INSCRIPCIONES");
                entity.Property(e => e.CantidadReprobados).HasColumnName("CANTIDAD_REPROBADOS");
                entity.Property(e => e.CantidadAprobados).HasColumnName("CANTIDAD_APROBADOS");
                entity.Property(e => e.CantidadGraduados).HasColumnName("CANTIDAD_GRADUADOS");
                entity.Property(e => e.TotalIngresoAdm).HasColumnName("TOTAL_INGRESO_ADM");
                entity.Property(e => e.TotalIngresoCambio).HasColumnName("TOTAL_INGRESO_CAMBIO");
                
            });
            
            
            modelBuilder.Entity<VW_MetricasCarreraPeriodo>(entity =>
            {
                entity.ToView("VW_METRICAS_CARRERA_PERIODO", "dbo");
                entity.HasNoKey();

                
                entity.Property(e => e.NombrePeriodo).HasColumnName("NOMBRE_PERIODO");
                entity.Property(e => e.Anio).HasColumnName("ANIO");
                entity.Property(e => e.Termino).HasColumnName("TERMINO");
                entity.Property(e => e.CodCarrera).HasColumnName("COD_CARRERA");
                entity.Property(e => e.NombreCarrera).HasColumnName("NOMBRE_CARRERA");
                entity.Property(e => e.CantidadEstudiantes).HasColumnName("CANTIDAD_ESTUDIANTES");
                entity.Property(e => e.CantidadAprobados).HasColumnName("CANTIDAD_APROBADOS");
                entity.Property(e => e.CantidadReprobados).HasColumnName("CANTIDAD_REPROBADOS");
                entity.Property(e => e.CantidadInscripciones).HasColumnName("CANTIDAD_INSCRIPCIONES");
                entity.Property(e => e.TotalIngresoAdm).HasColumnName("TOTAL_INGRESO_ADM");
                entity.Property(e => e.TotalIngresoCambio).HasColumnName("TOTAL_INGRESO_CAMBIO");

            });

            modelBuilder.Entity<VW_MetricasMateria>(entity =>
            {
                entity.ToView("VW_METRICAS_MATERIA", "dbo");
                entity.HasNoKey();

                
                entity.Property(e => e.NombrePeriodo).HasColumnName("NOMBRE_PERIODO");
                entity.Property(e => e.Anio).HasColumnName("ANIO");
                entity.Property(e => e.Termino).HasColumnName("TERMINO");
                entity.Property(e => e.CodCarrera).HasColumnName("COD_CARRERA");
                entity.Property(e => e.NombreCarrera).HasColumnName("NOMBRE_CARRERA");
                entity.Property(e => e.CodMateria).HasColumnName("COD_MATERIA");
                entity.Property(e => e.NombreMateria).HasColumnName("NOMBRE_MATERIA");
                entity.Property(e => e.CantidadEstudiantes).HasColumnName("CANTIDAD_ESTUDIANTES");
                entity.Property(e => e.CantidadInscripciones).HasColumnName("CANTIDAD_INSCRIPCIONES");
                entity.Property(e => e.CantidadAprobados).HasColumnName("CANTIDAD_APROBADOS");
                entity.Property(e => e.CantidadReprobados).HasColumnName("CANTIDAD_REPROBADOS");
                entity.Property(e => e.NivelCarrera).HasColumnName("NIVEL_CARRERA");
                entity.Property(e => e.PromedioMateria).HasColumnName("PROMEDIO_MATERIA");
                entity.Property(e => e.CodTipoCredito).HasColumnName("COD_TIPO_CREDITO");

            });

            modelBuilder.Entity<MallaStatsView>()
                .HasNoKey()
                .ToView("MallaStatsView");
        }


    }
}