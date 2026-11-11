using Library.Api.Common.Utils;

namespace Library.Api.Tests.Common.Utils;

public class TextNormalizerTests
{
    [Theory] // Indica que é um teste parametrizado, executado uma vez para cada conjunto de dados fornecido
    [InlineData("  ana DE souza ", "Ana de Souza")] // Parâmetros utilizados em cada execução do teste
    [InlineData("JOÃO DOS SANTOS", "João dos Santos")] // Cada InlineData gera uma execução indivídual de testes
    [InlineData("maria", "Maria")]
    public void NormalizeName_CapitalizesWordsAndKeepsParticlesLowercase(string input, string expected)
    {
        // Verifica se o valor inserido (input) é igual ao valor esperado (expected). Se forem diferentes, o teste falha
        Assert.Equal(expected, TextNormalizer.NormalizeName(input)); // Assert verifica se a lógica produziu o resultado esperado
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NormalizeName_WithBlankInput_ReturnsEmpty(string input)
    {
        Assert.Equal(string.Empty, TextNormalizer.NormalizeName(input));
    }

    [Theory]
    [InlineData("  Ana@Example.COM ", "ana@example.com")]
    [InlineData("JOAO@X.COM", "joao@x.com")]
    public void NormalizeEmail_TrimsAndLowercases(string input, string expected)
    {
        Assert.Equal(expected, TextNormalizer.NormalizeEmail(input));
    }
}