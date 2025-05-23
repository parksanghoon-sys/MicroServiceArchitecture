namespace Auth.Service.ApiModels;

public record LoginResponseDto(UserDto User, string Token);
