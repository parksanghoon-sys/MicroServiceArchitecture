namespace AuthApiService.Models;

/// <summary>
/// 토큰 응답을 나타내는 레코드
/// </summary>
/// <param name="AccessToken">액세스 토큰</param>
/// <param name="RefreshToken">리프레시 토큰</param>
/// <param name="ExpiresIn">만료 시간(초)</param>
/// <param name="TokenType">토큰 타입</param>
public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType = "Bearer"
);
