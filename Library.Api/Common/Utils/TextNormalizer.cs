namespace Library.Api.Common.Utils;

public static class TextNormalizer
{
    private static readonly HashSet<string> Particles =
        ["de", "da", "do", "das", "dos", "e"];
    
    public static string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var words = name.Trim()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return string.Join(" ", words.Select((word, index) =>
        {
            var normalized = word.ToLowerInvariant();

            if (index > 0 && Particles.Contains(normalized))
                return normalized;

            return char.ToUpperInvariant(normalized[0]) + normalized[1..];
        }));
    }

    public static string NormalizeEmail(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

}