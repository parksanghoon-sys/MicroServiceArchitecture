namespace AuthApiService.Configuration;

/// <summary>
/// JWT 설정
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// 설정 섹션 이름
    /// </summary>
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// JWT 서명에 사용할 비밀 키
    /// </summary>
    public string SecretKey { get; init; } = string.Empty;

    /// <summary>
    /// 토큰 발급자
    /// </summary>
    public string Issuer { get; init; } = string.Empty;

    /// <summary>
    /// 토큰 대상자
    /// </summary>
    public string Audience { get; init; } = string.Empty;

    /// <summary>
    /// 액세스 토큰 만료 시간 (분)
    /// </summary>
    public int AccessTokenExpirationMinutes { get; init; } = 15;

    /// <summary>
    /// 리프레시 토큰 만료 시간 (일)
    /// </summary>
    public int RefreshTokenExpirationDays { get; init; } = 7;

    /// <summary>
    /// 토큰 갱신 허용 시간 (시간)
    /// </summary>
    public int RefreshTokenRenewalHours { get; init; } = 24;
}
