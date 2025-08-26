namespace ApiToDo.Models;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty; // em produção, guarde hash
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }
    public bool IsActive => Revoked == null && !IsExpired;

    // Relação
    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    // Optional: link to a replacement token in rotation
    public string? ReplacedByToken { get; set; }
}
