namespace AuthApiService.Configuration;

/// <summary>
/// 보안 설정
/// </summary>
public class SecuritySettings
{
    /// <summary>
    /// 설정 섹션 이름
    /// </summary>
    public const string SectionName = "SecuritySettings";

    /// <summary>
    /// 비밀번호 최소 길이
    /// </summary>
    public int MinPasswordLength { get; init; } = 8;

    /// <summary>
    /// 비밀번호에 특수문자 필요 여부
    /// </summary>
    public bool RequireSpecialCharacter { get; init; } = true;

    /// <summary>
    /// 비밀번호에 숫자 필요 여부
    /// </summary>
    public bool RequireDigit { get; init; } = true;

    /// <summary>
    /// 비밀번호에 대문자 필요 여부
    /// </summary>
    public bool RequireUppercase { get; init; } = true;

    /// <summary>
    /// 비밀번호에 소문자 필요 여부
    /// </summary>
    public bool RequireLowercase { get; init; } = true;

    /// <summary>
    /// 로그인 시도 제한 횟수
    /// </summary>
    public int MaxLoginAttempts { get; init; } = 5;

    /// <summary>
    /// 계정 잠금 시간 (분)
    /// </summary>
    public int AccountLockoutMinutes { get; init; } = 30;

    /// <summary>
    /// HTTPS 강제 사용 여부
    /// </summary>
    public bool RequireHttps { get; init; } = true;

    /// <summary>
    /// CORS 허용 도메인
    /// </summary>
    public string[] AllowedOrigins { get; init; } = Array.Empty<string>();

    /// <summary>
    /// API 키 검증 여부
    /// </summary>
    public bool RequireApiKey { get; init; } = false;

    /// <summary>
    /// 허용된 API 키 목록
    /// </summary>
    public string[] ApiKeys { get; init; } = Array.Empty<string>();
}
