using System.Threading.Tasks;
using KoopaBackend.Domain.Entities;

namespace KoopaBackend.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario);
        Task<Usuario> CreateAsync(Usuario usuario);
        Task<bool> ExistsAsync(string nombreUsuario);
    }
}

