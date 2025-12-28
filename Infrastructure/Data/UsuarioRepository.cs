using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Infrastructure.Data;

namespace KoopaBackend.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly KoopaDbContext _context;

        public UsuarioRepository(KoopaDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> GetByNombreUsuarioAsync(string nombreUsuario)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> ExistsAsync(string nombreUsuario)
        {
            return await _context.Usuarios
                .AnyAsync(u => u.NombreUsuario == nombreUsuario);
        }
    }
}

