namespace Auth.Service.ApiModels;

public record RegistrationRequestDto(string UserId, string Email, string UserName, string? PhoneNumber, string Password, string? Role);
