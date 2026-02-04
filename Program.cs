using KoopaBackend.Infrastructure.Data;         
using KoopaBackend.Infrastructure.Repositories; 
using KoopaBackend.Domain.Interfaces;           
using KoopaBackend.Application.Services;        
using IBM.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);


// Agregar soporte para Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ==================================================================
// 1. CONFIGURACIÓN DE BASE DE DATOS (DbContext)
// ==================================================================
builder.Services.AddDbContext<KoopaDbContext>(options =>
    options.UseDb2(builder.Configuration.GetConnectionString("DefaultConnection"),
    p => p.SetServerInfo(IBMDBServerType.LUW))); // LUW = Linux/Unix/Windows (Versión de Docker)

// Inyección de dependencias
builder.Services.AddScoped<IMateriaRepository, MateriaRepository>();
builder.Services.AddScoped<IInscripcionesRepository, InscripcionesRepository>(); // Corregido a Singular según la entidad
builder.Services.AddScoped<ICarreraRepository, CarreraRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IFacultadRepository, FacultadRepository>();
builder.Services.AddScoped<IPlanificacionRepository, PlanificacionRepository>();
builder.Services.AddScoped<IFiltroRepository, FiltroRepository>();

// Repositorios de Tablas Puente y Catálogos
builder.Services.AddScoped<IMateriaCarreraRepository, MateriaCarreraRepository>();
builder.Services.AddScoped<IRequisitoRepository, RequisitoRepository>();
builder.Services.AddScoped<ITipoCreditoRepository, TipoCreditoRepository>();
builder.Services.AddScoped<ITipoRequisitoRepository, TipoRequisitoRepository>();


builder.Services.AddScoped<MateriaService>();
builder.Services.AddScoped<InscripcionesService>(); // Corregido a Singular
builder.Services.AddScoped<CarreraService>();
builder.Services.AddScoped<EstudianteService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<PlanificacionService>();
builder.Services.AddScoped<MateriaCarreraService>();
builder.Services.AddScoped<RequisitoService>();
builder.Services.AddScoped<TipoCreditoService>();
builder.Services.AddScoped<TipoRequisitoService>();
builder.Services.AddScoped<FiltroService>();


// Program.cs
builder.Services.AddScoped<IMetricasRepository, MetricasRepository>();
builder.Services.AddScoped<MetricasService>();

// ==================================================================
// 2. INYECCIÓN DE DEPENDENCIAS
// ==================================================================
// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("*") // frontend
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();
app.UseCors("AllowFrontend");


app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Koopa API V1");
    c.RoutePrefix = String.Empty;
});

app.UseHttpsRedirection();

// Aquí se habilitan los controladores
app.MapControllers();

app.Run();
