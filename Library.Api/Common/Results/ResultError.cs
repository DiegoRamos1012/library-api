namespace Library.Api.Common.Results;

public record ResultError(string Code, string Message, IReadOnlyList<string>? Details = null);