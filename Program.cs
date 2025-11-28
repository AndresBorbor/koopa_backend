using KoopaBackend.Infrastructure.Data;         // Para KoopaDbContext
using KoopaBackend.Infrastructure.Repositories; // Para MateriaRepository, InscripcionesRepository...
using KoopaBackend.Domain.Interfaces;           // Para IMateriaRepository...
using KoopaBackend.Application.Services;        // Para MateriaService...
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
// Asegúrate de instalar el paquete NuGet de tu base de datos (ej: IBM.EntityFrameworkCore para DB2)
// Aquí uso SQL Server como ejemplo, cámbialo por tu proveedor.
builder.Services.AddDbContext<KoopaDbContext>(options =>
    options.UseDb2(builder.Configuration.GetConnectionString("DefaultConnection"),
    p => p.SetServerInfo(IBMDBServerType.LUW))); // LUW = Linux/Unix/Windows (Versión de Docker)

// Inyección de dependencias
builder.Services.AddScoped<IMateriaRepository, MateriaRepository>();
builder.Services.AddScoped<IInscripcionesRepository, InscripcionesRepository>(); // Corregido a Singular según la entidad
builder.Services.AddScoped<ICarreraRepository, CarreraRepository>();
builder.Services.AddScoped<ISemestreRepository, SemestreRepository>();
builder.Services.AddScoped<IEstudianteRepository, EstudianteRepository>();
builder.Services.AddScoped<IFacultadRepository, FacultadRepository>();
builder.Services.AddScoped<IPlanificacionRepository, PlanificacionRepository>();
builder.Services.AddScoped<IProgramaRepository, ProgramaRepository>();

// Repositorios de Tablas Puente y Catálogos
builder.Services.AddScoped<IMateriaCarreraRepository, MateriaCarreraRepository>();
builder.Services.AddScoped<IRequisitoRepository, RequisitoRepository>();
builder.Services.AddScoped<ITipoCreditoRepository, TipoCreditoRepository>();
builder.Services.AddScoped<ITipoRequisitoRepository, TipoRequisitoRepository>();


builder.Services.AddScoped<MateriaService>();
builder.Services.AddScoped<InscripcionesService>(); // Corregido a Singular
builder.Services.AddScoped<CarreraService>();
builder.Services.AddScoped<SemestreService>();
builder.Services.AddScoped<EstudianteService>();
builder.Services.AddScoped<PlanificacionService>();
builder.Services.AddScoped<ProgramaService>();
builder.Services.AddScoped<MateriaCarreraService>();
builder.Services.AddScoped<RequisitoService>();
builder.Services.AddScoped<TipoCreditoService>();
builder.Services.AddScoped<TipoRequisitoService>();


// ==================================================================
// 2. INYECCIÓN DE DEPENDENCIAS (Lo más importante)
// ==================================================================
// Aquí le dices a .NET: "Cuando alguien pida IMateriaRepository, dale una instancia de MateriaRepository"
// builder.Services.AddScoped<IMateriaRepository, MateriaRepository>();

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
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate(); // aplica automáticamente todas las migraciones
// }
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aquí se habilitan los controladores
app.MapControllers();

app.Run();
