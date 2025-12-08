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
        public DbSet<Semestre> Semestres { get; set; }
        public DbSet<TipoCredito> TiposCredito { get; set; }
        public DbSet<TipoRequisito> TiposRequisito { get; set; }
        public DbSet<Materia> Materias { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }

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
        public DbSet<Programa> Programas { get; set; }

        public DbSet<MallaStatsView> MallaStatsViews { get; set; }
        

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

            modelBuilder.Entity<MallaStatsView>(e =>
            {
                e.HasNoKey(); // Es vista
                e.ToView("VW_MATERIAS_MALLA", "dbo"); // Tu nombre de vista y esquema

                // 👇 AQUÍ ESTÁ LA SOLUCIÓN: Mapeo explícito propiedad -> columna
                e.Property(v => v.CodCarrera).HasColumnName("COD_CARRERA");
                e.Property(v => v.CodMateria).HasColumnName("COD_MATERIA");
                e.Property(v => v.NombreMateria).HasColumnName("NOMBRE_MATERIA");
                e.Property(v => v.NivelCarrera).HasColumnName("NIVEL_CARRERA"); // Revisa si en tu vista es "NIVEL" o "NIVEL_CARRERA"
                
                e.Property(v => v.CodSemestre).HasColumnName("COD_SEMESTRE");
                e.Property(v => v.NombreSemestre).HasColumnName("NOMBRE_SEMESTRE");
                
                e.Property(v => v.InscritosActuales).HasColumnName("INSCRITOS_ACTUALES");
                e.Property(v => v.ReprobadosSemestreAnterior).HasColumnName("REPROBADOS_SEMESTRE_ANTERIOR");
                e.Property(v => v.HabilitadosParaTomarla).HasColumnName("HABILITADOS_PARA_TOMARLA");
            });

        }


    }
}