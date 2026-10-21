using System.ComponentModel.DataAnnotations;

namespace Library.Api.Modules.Users.Dtos.Request;

public record ChangeUserNameRequest(
    [Required, StringLength(150, MinimumLength = 2)] string NewFullName);