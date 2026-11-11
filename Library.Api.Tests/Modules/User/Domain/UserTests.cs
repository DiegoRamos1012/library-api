using Library.Api.Common.Exceptions;
using Library.Api.Modules.Users.Domain.Enums;

namespace Library.Api.Tests.Modules.User.Domain;

public class UserTests
{
    [Fact] // Teste unitário individual, executado uma só vez
    public void Create_WithMessyInput_NormalizesNameAndEmail()
    {
        var user = Api.Modules.Users.Domain.User.Create("  ana DE souza ", "  Ana@Example.COM ", "hash", UserRole.Employee);

        // TextNormalizer agindo para tratar os dados recebidos
        Assert.Equal("Ana de Souza", user.FullName);
        Assert.Equal("ana@example.com", user.Email);
        Assert.True(user.IsActive);
    }

    [Fact]
    public void Create_WithBlankName_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            Api.Modules.Users.Domain.User.Create("   ", "ana@example.com", "hash", UserRole.Employee));
    }
}