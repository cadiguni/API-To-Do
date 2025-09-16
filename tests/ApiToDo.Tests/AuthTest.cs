using Xunit;
using ApiToDo.Services;
using ApiToDo.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;

namespace ApiToDo.Tests
{
    public class AuthTests
    {
        private readonly TokenService _tokenService;

        public AuthTests()
        {
            // 🔑 Chave fixa para os testes (não usar em produção)
            var inMemorySettings = new Dictionary<string, string?> {
                {"Jwt:Key", "omc7m1lcezr7vkvld8frhwt7va206imqrq"}
            };
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
            _tokenService = new TokenService(configuration);
        }

        [Fact]
        public void DeveGerarRefreshTokenValido()
        {
            // Act
            var refresh = _tokenService.GenerateRefreshToken();

            // Assert
            Assert.False(string.IsNullOrEmpty(refresh.Token));
            Assert.True(refresh.Expires > DateTime.UtcNow);
        }

        [Fact]
        public void DeveGerarAccessTokenValido()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Username = "teste",
                Role = "user"
            };

            // Act
            var access = _tokenService.GenerateAccessToken(usuario);

            // Assert
            Assert.False(string.IsNullOrEmpty(access));
            Assert.Contains("eyJ", access); // Tokens JWT sempre começam com "eyJ"
        }
    }
}
