namespace AuthApiService.Configuration;

/// <summary>
/// 애플리케이션 설정
/// </summary>
public class AppSettings
{
    /// <summary>
    /// 설정 섹션 이름
    /// </summary>
    public const string SectionName = "AppSettings";

    /// <summary>
    /// 애플리케이션 이름
    /// </summary>
    public string ApplicationName { get; init; } = "AuthApiService";

    /// <summary>
    /// 애플리케이션 버전
    /// </summary>
    public string Version { get; init; } = "1.0.0";

    /// <summary>
    /// 환경 (Development, Staging, Production)
    /// </summary>
    public string Environment { get; init; } = "Development";

    /// <summary>
    /// API 문서화 활성화 여부
    /// </summary>
    public bool EnableSwagger { get; init; } = true;

    /// <summary>
    /// 상태 검사 활성화 여부
    /// </summary>
    public bool EnableHealthChecks { get; init; } = true;

    /// <summary>
    /// 메트릭 수집 활성화 여부
    /// </summary>
    public bool EnableMetrics { get; init; } = true;

    /// <summary>
    /// 개발자 예외 페이지 활성화 여부
    /// </summary>
    public bool EnableDeveloperExceptionPage { get; init; } = false;

    /// <summary>
    /// 요청 로깅 활성화 여부
    /// </summary>
    public bool EnableRequestLogging { get; init; } = true;

    /// <summary>
    /// 응답 압축 활성화 여부
    /// </summary>
    public bool EnableResponseCompression { get; init; } = true;

    /// <summary>
    /// 캐싱 활성화 여부
    /// </summary>
    public bool EnableCaching { get; init; } = true;
}
