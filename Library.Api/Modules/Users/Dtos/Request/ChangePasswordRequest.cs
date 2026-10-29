namespace Library.Api.Modules.Users.Dtos.Request;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);