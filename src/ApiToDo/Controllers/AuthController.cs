using ApiToDo.Data;
using ApiToDo.Models;
using ApiToDo.Models.Auth;
using ApiToDo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpPost("registrar")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registro)
    {
        var usuario = new Usuario
        {
            Username = registro.Username,
            Senha = registro.Senha,
            Role = registro.Role
        };
        // Em produção: validar, checar username único e hashear senha
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Usuário registrado" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest credenciais)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Username == credenciais.Username && u.Senha == credenciais.Senha);

        if (usuario == null) return Unauthorized("Usuário ou senha inválidos");

        var accessToken = _tokenService.GenerateAccessToken(usuario);
        var refreshToken = _tokenService.GenerateRefreshToken();

        // Salva refresh token
        usuario.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return Ok(new {
            accessToken,
            refreshToken = refreshToken.Token
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] string token)
    {
        if (string.IsNullOrEmpty(token)) return BadRequest("Token inválido");

        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.Usuario)
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (refreshToken == null || !refreshToken.IsActive)
            return Unauthorized("Refresh token inválido ou expirado");

        // opcional: revogar token antigo e criar novo (rotation)
        refreshToken.Revoked = DateTime.UtcNow;

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        refreshToken.ReplacedByToken = newRefreshToken.Token;
        refreshToken.Usuario!.RefreshTokens.Add(newRefreshToken);

        // gerar novo access token
        var newAccessToken = _tokenService.GenerateAccessToken(refreshToken.Usuario);

        await _context.SaveChangesAsync();

        return Ok(new {
            accessToken = newAccessToken,
            refreshToken = newRefreshToken.Token
        });
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] string token)
    {
        var refreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        if (refreshToken == null) return NotFound();

        refreshToken.Revoked = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok("Revogado");
    }
}
