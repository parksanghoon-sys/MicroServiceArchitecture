namespace Auth.Service.ApiModels.Requests;

public record RegistrationRequestDto(string UserId, string Email, string UserName, string? PhoneNumber, string Password, string? Role);
