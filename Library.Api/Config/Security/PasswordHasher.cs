namespace Library.Api.Config.Security;

using BCrypt.Net;

public class PasswordHasher
{
    private const int WorkFactor = 12;
    
    public string PasswordEncoder(string password)
    {
        var passwordEncoded = BCrypt.HashPassword( password, WorkFactor); // "workFactor" define o custo computacional do BCrypt para gerar e verificar hashes de senhas.
        return passwordEncoded;
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Verify(password, passwordHash);
    }
}