namespace Auth.Service.ApiModels.Responses;

public record LoginResponseDto(UserDto? User, TokenResponseDto Token);
