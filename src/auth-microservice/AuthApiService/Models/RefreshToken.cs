namespace AuthApiService.Models;

/// <summary>
/// 리프레시 토큰 정보를 나타내는 레코드
/// </summary>
/// <param name="Id">고유 식별자</param>
/// <param name="UserId">사용자 ID</param>
/// <param name="Token">토큰 값</param>
/// <param name="ExpiresAt">만료 시간</param>
/// <param name="CreatedAt">생성 시간</param>
/// <param name="IsRevoked">폐기 여부</param>
public record RefreshToken(
    int Id,
    int UserId,
    string Token,
    DateTime ExpiresAt,
    DateTime CreatedAt,
    bool IsRevoked
)
{
    /// <summary>
    /// 새 리프레시 토큰 생성을 위한 정적 팩토리 메서드
    /// </summary>
    /// <param name="userId">사용자 ID</param>
    /// <param name="token">토큰 값</param>
    /// <param name="expiresAt">만료 시간</param>
    /// <returns>새로운 RefreshToken 인스턴스</returns>
    public static RefreshToken Create(int userId, string token, DateTime expiresAt)
        => new(0, userId, token, expiresAt, DateTime.UtcNow, false);

    /// <summary>
    /// 토큰이 유효한지 확인
    /// </summary>
    public bool IsValid => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}
