namespace AuthApiService.Models;

/// <summary>
/// 사용자 등록 요청을 나타내는 레코드
/// </summary>
/// <param name="Username">사용자명</param>
/// <param name="Email">이메일</param>
/// <param name="Password">비밀번호</param>
public record RegisterRequest(string Username, string Email, string Password);
