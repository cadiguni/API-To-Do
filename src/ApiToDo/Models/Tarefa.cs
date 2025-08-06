namespace ApiToDo.Models;

public class Tarefa
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public bool Concluida { get; set; } = false;
    public DateTime DataCriacao { get; set; } = DateTime.Now;
}
