using System.Security.Claims;
using System.Text;
using Library.Api.Config.Security;
using Library.Api.Modules.Users.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Library.Api.Modules.Auth.Services;

public class JwtTokenService(IOptions<JwtOptions> options, TimeProvider timeProvider)
{
    private readonly JwtOptions _options = options.Value;

    public string GenerateToken(User user)
    {
        var now = timeProvider.GetUtcNow();

        var descriptor = new SecurityTokenDescriptor
        {
            // Claims são os dados dentro do token
            Subject = new ClaimsIdentity([
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            ]),
            // Diz quem emitiu o Token
            Issuer = _options.Issuer,
            // Diz pra quem o token vale
            Audience = _options.Audience,
            IssuedAt = now.UtcDateTime,
            // Limita o tempo de vida do token (60 minutos, como definido pelo TimeProvider)
            Expires = now.AddMinutes(_options.ExpirationMinutes).UtcDateTime,
            // Assinatura do token
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret)),
                SecurityAlgorithms.HmacSha256)
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }
}