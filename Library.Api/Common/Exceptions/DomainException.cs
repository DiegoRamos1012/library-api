namespace Library.Api.Common.Exceptions;

public class DomainException : Exception
{
    // Erro de domínio genérico
    public DomainException() : base("Domain rule violated") {}

    // Erro de domínio com mensagem personalizada
    public DomainException(string message) : base(message) {}
}

