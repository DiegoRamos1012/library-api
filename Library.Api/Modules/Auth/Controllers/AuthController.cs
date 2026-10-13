using Library.Api.Common.Controllers;
using Library.Api.Modules.Auth.Dtos.Request;
using Library.Api.Modules.Auth.Dtos.Response;
using Library.Api.Modules.Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Modules.Auth.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(AuthService authService, JwtTokenService jwtTokenService) : ApiControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        
        if (result.IsFailure)
            return ToProblem(result.Error);

        var token = jwtTokenService.GenerateToken(result.Value);

        return Ok(new LoginResponse(token));
    }
}