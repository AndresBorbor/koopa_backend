using KoopaBackend.Application.Services;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Agregar soporte para Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inyección de dependencias
builder.Services.AddScoped<IMateriaRepository, MateriaRepository>();
builder.Services.AddScoped<MateriaService>();


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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Aquí se habilitan los controladores
app.MapControllers();

app.Run();
