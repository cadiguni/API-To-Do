namespace ApiToDo.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty; // para estudo; em produção, armazene hash
    public string Role { get; set; } = "user"; // roles: "user", "admin", etc.
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}
