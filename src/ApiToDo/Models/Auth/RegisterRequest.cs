namespace ApiToDo.Models.Auth
{
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // opcional, se quiser atribuir no registro
    }
}
