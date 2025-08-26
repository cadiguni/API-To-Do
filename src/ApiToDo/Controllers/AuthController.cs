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
        if (_context.Usuarios.Any(u => u.Username == registro.Username))
            return BadRequest(new { message = "Usuário já existe" });

        var usuario = new Usuario
        {
            Username = registro.Username,
            // senha agora é um HASH
            Senha = BCrypt.Net.BCrypt.HashPassword(registro.Senha),
            Role = registro.Role ?? "user"
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Usuário registrado com sucesso" });
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest credenciais)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Username == credenciais.Username);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(credenciais.Senha, usuario.Senha))
            return Unauthorized(new { message = "Usuário ou senha inválidos" });

        var token = _tokenService.GenerateAccessToken(usuario);
        var refreshToken = _tokenService.GenerateRefreshToken();

        usuario.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            AccessToken = token,
            RefreshToken = refreshToken.Token
        });
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest body)
    {
        if (string.IsNullOrWhiteSpace(body.RefreshToken))
            return BadRequest(new { message = "Refresh token requerido" });

        var rt = await _context.RefreshTokens
            .Include(x => x.Usuario)
            .FirstOrDefaultAsync(x => x.Token == body.RefreshToken);

        if (rt == null || rt.IsExpired || rt.Revoked != null)
            return Unauthorized(new { message = "Refresh token inválido" });

        // revoga o antigo e cria um novo (rotation)
        rt.Revoked = DateTime.UtcNow;
        var newRt = _tokenService.GenerateRefreshToken();
        rt.ReplacedByToken = newRt.Token;
        rt.Usuario!.RefreshTokens.Add(newRt);

        var newAccess = _tokenService.GenerateAccessToken(rt.Usuario);

        await _context.SaveChangesAsync();

        return Ok(new { accessToken = newAccess, refreshToken = newRt.Token });
    }

  
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));

        if (usuario == null)
            return NotFound(new { message = "Refresh token não encontrado" });

        var token = usuario.RefreshTokens.First(rt => rt.Token == refreshToken);
        token.Revoked = DateTime.UtcNow; // marca como inválido

        await _context.SaveChangesAsync();

        return Ok(new { message = "Logout realizado com sucesso" });
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
