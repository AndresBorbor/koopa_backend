using System.Collections.Generic;
using System.Threading.Tasks;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Application.DTOs; // Asegúrate de importar donde está el DTO

namespace KoopaBackend.Domain.Interfaces
{
    public interface IMateriaCarreraRepository
    {
        // Métodos básicos (opcionales si ya los tienes en un genérico)
        Task<IEnumerable<MateriaCarrera>> GetAllAsync();
    }
}