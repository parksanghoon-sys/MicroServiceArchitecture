namespace AuthApiService.Models;

/// <summary>
/// 리프레시 토큰 요청을 나타내는 레코드
/// </summary>
/// <param name="RefreshToken">리프레시 토큰</param>
public record RefreshTokenRequest(string RefreshToken);
