using System.Threading.Tasks;
using KoopaBackend.Domain.Entities;
using KoopaBackend.Domain.Interfaces;
using KoopaBackend.Application.DTOs;

namespace KoopaBackend.Application.Services
{
    public class UsuarioService
    {
        private readonly IUsuarioRepository _repository;

        public UsuarioService(IUsuarioRepository repository)
        {
            _repository = repository;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var usuario = await _repository.GetByNombreUsuarioAsync(request.NombreUsuario);

            if (usuario == null)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Usuario o contraseña incorrectos"
                };
            }

            // Verificar contraseña
            // Si la contraseña almacenada parece un hash de BCrypt, la validamos con BCrypt.
            // Si no, asumimos que es texto plano (usuarios antiguos) y comparamos directamente.
            bool isBCryptHash = !string.IsNullOrEmpty(usuario.Password) &&
                                usuario.Password.StartsWith("$2");

            bool isPasswordValid = isBCryptHash
                ? BCrypt.Net.BCrypt.Verify(request.Password, usuario.Password)
                : usuario.Password == request.Password;

            if (!isPasswordValid)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Usuario o contraseña incorrectos"
                };
            }

            return new LoginResponseDto
            {
                Success = true,
                Message = "Login exitoso",
                CodCarrera = usuario.CodCarrera,
                NombreUsuario = usuario.NombreUsuario
            };
        }

        public async Task<LoginResponseDto> CreateUserAsync(CreateUserRequestDto request)
        {
            // Verificar si el usuario ya existe
            bool exists = await _repository.ExistsAsync(request.NombreUsuario);
            if (exists)
            {
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "El nombre de usuario ya existe"
                };
            }

            // Hash de la contraseña
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var usuario = new Usuario
            {
                NombreUsuario = request.NombreUsuario,
                Password = hashedPassword,
                CodCarrera = request.CodCarrera
            };

            await _repository.CreateAsync(usuario);

            return new LoginResponseDto
            {
                Success = true,
                Message = "Usuario creado exitosamente",
                CodCarrera = usuario.CodCarrera,
                NombreUsuario = usuario.NombreUsuario
            };
        }
    }
}

