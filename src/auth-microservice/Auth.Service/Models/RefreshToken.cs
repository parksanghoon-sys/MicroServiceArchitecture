using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Auth.Service.Models;

[Owned]
public class RefreshToken
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Token { get; set; } // 리프레시 토큰 값

    [Required]
    public string UserId { get; set; } // 사용자 ID
    // 토큰 생성
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresAt { get; set; } // 만료 시간

    public bool IsRevoked { get; set; } = false; // 토큰 폐기 여부

    public string CreatedByIp { get; set; } // 생성된 IP

    public DateTime? RevokedAt { get; set; } // 폐기된 시간

    public string RevokedByIp { get; set; } // 폐기된 IP

    public string ReplacedByToken { get; set; } // 대체된 토큰

    // 토큰이 활성 상태인지 확인
    public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;

    // 사용자와의 관계
    public virtual ApplicationUser User { get; set; }
}
