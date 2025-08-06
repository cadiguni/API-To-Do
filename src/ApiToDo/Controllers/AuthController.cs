using ApiToDo.Data;
using ApiToDo.Models;
using ApiToDo.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiToDo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;

    public AuthController(AppDbContext context, TokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] Usuario credenciais)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u =>
            u.Username == credenciais.Username && u.Senha == credenciais.Senha);

        if (usuario == null) return Unauthorized("Usuário ou senha inválidos");

        var token = _tokenService.GenerateToken(usuario);
        return Ok(new { token });
    }

    [HttpPost("registrar")]
    public IActionResult Registrar([FromBody] Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
        return Ok("Usuário registrado com sucesso");
    }
}
