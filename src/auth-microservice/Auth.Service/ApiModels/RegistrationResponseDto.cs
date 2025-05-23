namespace Auth.Service.ApiModels;

public record LoginSuccessResponseDto(object? Result, bool IsSuccess, string Message);
