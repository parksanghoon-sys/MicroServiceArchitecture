namespace AuthApiService.Configuration;

/// <summary>
/// 데이터베이스 설정
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// 설정 섹션 이름
    /// </summary>
    public const string SectionName = "DatabaseSettings";

    /// <summary>
    /// 연결 문자열
    /// </summary>
    public string ConnectionString { get; init; } = string.Empty;

    /// <summary>
    /// 데이터베이스 공급자
    /// </summary>
    public string Provider { get; init; } = "InMemory";

    /// <summary>
    /// 연결 풀 최대 크기
    /// </summary>
    public int MaxPoolSize { get; init; } = 100;

    /// <summary>
    /// 명령 타임아웃 (초)
    /// </summary>
    public int CommandTimeoutSeconds { get; init; } = 30;

    /// <summary>
    /// 자동 마이그레이션 활성화 여부
    /// </summary>
    public bool EnableAutoMigration { get; init; } = true;
}
