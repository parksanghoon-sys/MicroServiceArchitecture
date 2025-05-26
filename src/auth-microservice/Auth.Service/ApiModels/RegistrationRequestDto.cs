namespace Auth.Service.ApiModels;

public record RegistrationRequestDto(string Email, string FirstName,string LastName, string? PhoneNumber, string Password, string? Role);
