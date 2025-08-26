using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiToDo.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;


namespace ApiToDo.Services;

public class TokenService
{
    private readonly IConfiguration _config;
    private readonly TimeSpan _accessTokenValidity = TimeSpan.FromMinutes(15);
    private readonly TimeSpan _refreshTokenValidity = TimeSpan.FromDays(7);

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateAccessToken(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.Username),
            new Claim(ClaimTypes.Role, usuario.Role)
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"], // pode ser null se ValidateAudience=false
            claims: claims,
            expires: DateTime.UtcNow.Add(_accessTokenValidity),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        var token = Convert.ToBase64String(randomBytes);

        return new RefreshToken
        {
            Token = token,
            Expires = DateTime.UtcNow.Add(_refreshTokenValidity),
            Created = DateTime.UtcNow
        };
    }
}
