namespace Auth.Service.ApiModels;

public record RegistrationRequestDto(string Email, string Name, string PhoneNumber, string Password, string? Role);
