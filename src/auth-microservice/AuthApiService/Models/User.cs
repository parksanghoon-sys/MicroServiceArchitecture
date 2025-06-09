using Microsoft.AspNetCore.Identity;

namespace AuthApiService.Models;

/// <summary>
/// 사용자 정보를 나타내는 레코드
/// </summary>
/// <param name="Id">사용자 고유 식별자</param>
/// <param name="Username">사용자명</param>
/// <param name="Email">이메일 주소</param>
/// <param name="PasswordHash">해시된 비밀번호</param>
/// <param name="CreatedAt">생성 일시</param>
/// <param name="IsActive">활성 상태</param>
public class ApplicationUser: IdentityUser
{
    public int UserId;
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }
    public string LastLoginIp { get; set; }

    public string RegistrationIp { get; set; }

    public bool IsActive { get; set; } = true;
    public string Username { get; set; } = string.Empty;

    public static ApplicationUser Create(string userId, string email, string passwordHash)
        => new ApplicationUser()
        {
            PasswordHash = passwordHash,
            Email = email,
            UserName = userId
        };
}
