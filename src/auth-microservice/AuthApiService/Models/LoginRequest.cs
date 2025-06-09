namespace AuthApiService.Models;

/// <summary>
/// 로그인 요청을 나타내는 레코드
/// </summary>
/// <param name="Username">사용자명 또는 이메일</param>
/// <param name="Password">비밀번호</param>
public record LoginRequest(string Username, string Password);
