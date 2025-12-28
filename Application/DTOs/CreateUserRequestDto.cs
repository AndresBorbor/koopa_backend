namespace KoopaBackend.Application.DTOs
{
    public class CreateUserRequestDto
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? CodCarrera { get; set; }
    }
}

