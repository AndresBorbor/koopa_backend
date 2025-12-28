namespace KoopaBackend.Application.DTOs
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? CodCarrera { get; set; }
        public string? NombreUsuario { get; set; }
    }
}

