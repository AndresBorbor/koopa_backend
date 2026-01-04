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
    public class FiltroRepository : IFiltroRepository
    {
        private readonly KoopaDbContext _context;

        public FiltroRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string,IEnumerable<FiltroDto>>> GetFiltros()
        {
            // 1. Await (espera) para obtener la lista de Carreras
            var carreras = await _context.Carreras
                .AsNoTracking()
                .Select(c => new FiltroDto { Id = c.CodCarrera.ToString(), Value = c.NombreCarrera })
                .ToListAsync(); // El ToListAsync() ahora se ejecuta sobre la proyección de LINQ
            carreras.Insert(0, new FiltroDto { Id = "all", Value = "Todos" });

            // 2. Await (espera) para obtener la lista de Semestres
            var periodos = await _context.Periodos
                .AsNoTracking()
                .Select(s => new FiltroDto { Id = s.Anio.ToString() + s.Termino.ToString(), Value  = s.NombrePeriodo})
                .ToListAsync(); // El ToListAsync() ahora se ejecuta sobre la proyección de LINQ
            periodos.Insert(0, new FiltroDto { Id = "Todos", Value = "all" });
            // Mapear a Diccionario
            var resultado = new Dictionary<string, IEnumerable<FiltroDto>>
            {
                { "carreras", carreras },
                { "periodos", periodos }
            };

            return resultado;
        }
    }
}