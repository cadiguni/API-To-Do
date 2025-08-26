using ApiToDo.Data;
using ApiToDo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiToDo.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "user,admin")]
public class TarefasController : ControllerBase
{
    private readonly AppDbContext _context;

    public TarefasController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Listar() => Ok(_context.Tarefas.ToList());

    [HttpPost]
    public IActionResult Criar([FromBody] Tarefa tarefa)
    {
        _context.Tarefas.Add(tarefa);
        _context.SaveChanges();
        return Created("", tarefa);
    }

    [HttpGet("{id}")]
    public IActionResult Obter(int id)
    {
        var tarefa = _context.Tarefas.Find(id);
        if (tarefa == null) return NotFound();
        return Ok(tarefa);
    }

    [HttpPut("{id}")]
    public IActionResult Atualizar(int id, [FromBody] Tarefa nova)
    {
        var tarefa = _context.Tarefas.Find(id);
        if (tarefa == null) return NotFound();

        tarefa.Titulo = nova.Titulo;
        tarefa.Descricao = nova.Descricao;
        tarefa.Concluida = nova.Concluida;

        _context.SaveChanges();
        return Ok(tarefa);
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id}")]
    public IActionResult Deletar(int id)
    {
        var tarefa = _context.Tarefas.Find(id);
        if (tarefa == null) return NotFound();

        _context.Tarefas.Remove(tarefa);
        _context.SaveChanges();
        return NoContent();
    }
}
