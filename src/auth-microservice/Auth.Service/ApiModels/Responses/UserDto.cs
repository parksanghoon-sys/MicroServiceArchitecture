namespace Auth.Service.ApiModels.Responses;

public record UserDto(string UserId, string Email, string Name, string? PhoneNumber);
