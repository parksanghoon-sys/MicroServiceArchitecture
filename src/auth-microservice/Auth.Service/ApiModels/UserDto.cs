namespace Auth.Service.ApiModels;

public record UserDto(string UserId, string Email, string Name, string? PhoneNumber);
