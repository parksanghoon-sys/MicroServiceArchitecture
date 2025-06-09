namespace AuthApiService.Configuration;

/// <summary>
/// 로깅 설정
/// </summary>
public class LoggingSettings
{
    /// <summary>
    /// 설정 섹션 이름
    /// </summary>
    public const string SectionName = "LoggingSettings";

    /// <summary>
    /// 로그 파일 경로
    /// </summary>
    public string LogFilePath { get; init; } = "logs/app.log";

    /// <summary>
    /// 최소 로그 레벨
    /// </summary>
    public string MinimumLevel { get; init; } = "Information";

    /// <summary>
    /// 콘솔 로깅 활성화 여부
    /// </summary>
    public bool EnableConsoleLogging { get; init; } = true;

    /// <summary>
    /// 파일 로깅 활성화 여부
    /// </summary>
    public bool EnableFileLogging { get; init; } = true;

    /// <summary>
    /// 구조화된 로깅 활성화 여부
    /// </summary>
    public bool EnableStructuredLogging { get; init; } = true;

    /// <summary>
    /// 로그 파일 최대 크기 (MB)
    /// </summary>
    public int MaxLogFileSizeMB { get; init; } = 100;

    /// <summary>
    /// 보관할 로그 파일 수
    /// </summary>
    public int RetainedFileCountLimit { get; init; } = 31;

    /// <summary>
    /// 민감한 데이터 로깅 방지 여부
    /// </summary>
    public bool PreventSensitiveDataLogging { get; init; } = true;
}
