namespace Library.Api.Common.Exceptions;

public class BusinessException : Exception
{
    public BusinessException() : base("Business rule violated") {}

    // Erro de domínio com mensagem personalizada
    public BusinessException(string message) : base(message) {}
    
    public class EmailAlreadyExistsException()
        : Exception("There is already a registered user with this email.");

    public class UserNotFoundException()
        : Exception("User not found");
}