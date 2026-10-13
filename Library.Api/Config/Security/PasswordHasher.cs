namespace Library.Api.Config.Security;

public class PasswordHasher
{
    private const int WorkFactor = 12;
    
    public string PasswordEncoder(string password)
    {
        var passwordEncoded = BCrypt.Net.BCrypt.EnhancedHashPassword( password, WorkFactor); // "workFactor" define o custo computacional do BCrypt para gerar e verificar hashes de senhas.
        return passwordEncoded;
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
    }
}