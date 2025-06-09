using AuthApiService.Interfaces;
using AuthApiService.Models;
using AuthApiService.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System.Collections.Concurrent;

namespace AuthApiService.Services;

/// <summary>
/// 구조화된 로깅 서비스 구현
/// </summary>
public class LoggingService : ILoggingService
{
    private readonly LoggingSettings _loggingSettings;
    private readonly Serilog.ILogger _logger;
    private readonly ConcurrentQueue<LogEntry> _logQueue;
    private readonly Timer _flushTimer;

    public LoggingService(IOptions<LoggingSettings> loggingSettings, Serilog. ILogger logger)
    {
        _loggingSettings = loggingSettings.Value;
        _logger = logger;
        _logQueue = new ConcurrentQueue<LogEntry>();
        
        // 배치 로깅을 위한 타이머 (5초마다 큐에 있는 로그들을 플러시)
        _flushTimer = new Timer(FlushLogs, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
    }

    /// <summary>
    /// 정보 로그 기록
    /// </summary>
    public async Task LogInformationAsync(
        string message,
        string category = "General",
        int? userId = null,
        Dictionary<string, object>? properties = null,
        CancellationToken cancellationToken = default)
    {
        var logEntry = LogEntry.Create(LogLevel.Information, message, category, userId, properties);
        await LogAsync(logEntry, cancellationToken);
    }

    /// <summary>
    /// 경고 로그 기록
    /// </summary>
    public async Task LogWarningAsync(
        string message,
        string category = "General",
        int? userId = null,
        Dictionary<string, object>? properties = null,
        CancellationToken cancellationToken = default)
    {
        var logEntry = LogEntry.Create(LogLevel.Warning, message, category, userId, properties);
        await LogAsync(logEntry, cancellationToken);
    }

    /// <summary>
    /// 에러 로그 기록
    /// </summary>
    public async Task LogErrorAsync(
        string message,
        Exception? exception = null,
        string category = "Error",
        int? userId = null,
        Dictionary<string, object>? properties = null,
        CancellationToken cancellationToken = default)
    {
        var logProperties = properties ?? new Dictionary<string, object>();
        
        if (exception != null)
        {
            logProperties["Exception"] = exception.ToString();
            logProperties["ExceptionType"] = exception.GetType().Name;
            logProperties["StackTrace"] = exception.StackTrace ?? "";
        }

        var logEntry = LogEntry.Create(LogLevel.Error, message, category, userId, logProperties);
        await LogAsync(logEntry, cancellationToken);
    }

    /// <summary>
    /// 디버그 로그 기록
    /// </summary>
    public async Task LogDebugAsync(
        string message,
        string category = "Debug",
        int? userId = null,
        Dictionary<string, object>? properties = null,
        CancellationToken cancellationToken = default)
    {
        var logEntry = LogEntry.Create(LogLevel.Debug, message, category, userId, properties);
        await LogAsync(logEntry, cancellationToken);
    }

    /// <summary>
    /// 인증 관련 로그 기록
    /// </summary>
    public async Task LogAuthenticationAsync(
        string action,
        string username,
        bool success,
        string? ipAddress = null,
        string? userAgent = null,
        Dictionary<string, object>? additionalInfo = null,
        CancellationToken cancellationToken = default)
    {
        var properties = additionalInfo ?? new Dictionary<string, object>();
        properties["Action"] = action;
        properties["Username"] = SanitizeUsername(username);
        properties["Success"] = success;
        properties["IpAddress"] = ipAddress ?? "Unknown";
        properties["UserAgent"] = SanitizeUserAgent(userAgent);
        properties["Timestamp"] = DateTime.UtcNow;

        var level = success ? LogLevel.Information : LogLevel.Warning;
        var message = success 
            ? $"인증 성공: {action} for user {SanitizeUsername(username)}"
            : $"인증 실패: {action} for user {SanitizeUsername(username)}";

        var logEntry = LogEntry.Create(level, message, "Authentication", null, properties);
        await LogAsync(logEntry, cancellationToken);
    }

    /// <summary>
    /// API 요청 로그 기록
    /// </summary>
    public async Task LogApiRequestAsync(
        string method,
        string path,
        int statusCode,
        TimeSpan duration,
        int? userId = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        var properties = new Dictionary<string, object>
        {
            ["Method"] = method,
            ["Path"] = SanitizePath(path),
            ["StatusCode"] = statusCode,
            ["Duration"] = duration.TotalMilliseconds,
            ["DurationString"] = $"{duration.TotalMilliseconds:F2}ms",
            ["IpAddress"] = ipAddress ?? "Unknown",
            ["UserAgent"] = SanitizeUserAgent(userAgent),
            ["Timestamp"] = DateTime.UtcNow
        };

        if (userId.HasValue)
        {
            properties["UserId"] = userId.Value;
        }

        var level = statusCode >= 400 ? LogLevel.Warning : LogLevel.Information;
        var message = $"{method} {SanitizePath(path)} responded {statusCode} in {duration.TotalMilliseconds:F2}ms";

        var logEntry = LogEntry.Create(level, message, "ApiRequest", userId, properties);
        await LogAsync(logEntry, cancellationToken);
    }

    /// <summary>
    /// 구조화된 로그 기록
    /// </summary>
    public async Task LogAsync(LogEntry logEntry, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(logEntry);

        // 민감한 데이터 필터링
        var sanitizedEntry = SanitizeLogEntry(logEntry);
        
        // 큐에 추가 (비동기 처리를 위해)
        _logQueue.Enqueue(sanitizedEntry);

        // Serilog를 통한 즉시 로깅
        await WriteToSerilogAsync(sanitizedEntry);
    }

    /// <summary>
    /// 로그 조회 (메모리 내 구현 - 실제 환경에서는 데이터베이스 사용)
    /// </summary>
    public async Task<IEnumerable<LogEntry>> GetLogsAsync(
        LogLevel? level = null,
        string? category = null,
        int? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        // 실제 구현에서는 데이터베이스나 로그 저장소에서 조회
        // 여기서는 시뮬레이션을 위한 빈 결과 반환
        await Task.Delay(10, cancellationToken); // 비동기 시뮬레이션
        
        return Enumerable.Empty<LogEntry>();
    }

    /// <summary>
    /// Serilog를 통한 로그 기록
    /// </summary>
    private async Task WriteToSerilogAsync(LogEntry logEntry)
    {
        await Task.Run(() =>
        {
            var properties = logEntry.Properties ?? new Dictionary<string, object>();
            properties["LogId"] = logEntry.Id;
            properties["Category"] = logEntry.Category;
            
            if (logEntry.UserId.HasValue)
            {
                properties["UserId"] = logEntry.UserId.Value;
            }

            var loggerContext = _logger
                .ForContext("Category", logEntry.Category)
                .ForContext("LogId", logEntry.Id);

            if (logEntry.UserId.HasValue)
            {
                loggerContext = loggerContext.ForContext("UserId", logEntry.UserId.Value);
            }

            foreach (var prop in properties)
            {
                loggerContext = loggerContext.ForContext(prop.Key, prop.Value);
            }

            switch (logEntry.Level)
            {
                case LogLevel.Debug:
                    loggerContext.Debug(logEntry.Message);
                    break;
                case LogLevel.Information:
                    loggerContext.Information(logEntry.Message);
                    break;
                case LogLevel.Warning:
                    loggerContext.Warning(logEntry.Message);
                    break;
                case LogLevel.Error:
                    loggerContext.Error(logEntry.Message);
                    break;
                case LogLevel.Critical:
                    loggerContext.Fatal(logEntry.Message);
                    break;
                default:
                    loggerContext.Information(logEntry.Message);
                    break;
            }
        });
    }

    /// <summary>
    /// 배치 로그 플러시
    /// </summary>
    private void FlushLogs(object? state)
    {
        var logCount = 0;
        var logs = new List<LogEntry>();

        while (_logQueue.TryDequeue(out var logEntry) && logCount < 100)
        {
            logs.Add(logEntry);
            logCount++;
        }

        if (logs.Any())
        {
            // 여기서 배치로 데이터베이스에 저장하거나 외부 로깅 시스템에 전송
            _ = Task.Run(async () =>
            {
                foreach (var log in logs)
                {
                    // 실제 구현에서는 데이터베이스 저장
                    await Task.Delay(1); // 시뮬레이션
                }
            });
        }
    }

    /// <summary>
    /// 민감한 데이터가 포함된 로그 엔트리를 정제
    /// </summary>
    private LogEntry SanitizeLogEntry(LogEntry logEntry)
    {
        if (!_loggingSettings.PreventSensitiveDataLogging)
            return logEntry;

        var sanitizedProperties = logEntry.Properties?.ToDictionary(
            kvp => kvp.Key,
            kvp => SanitizeValue(kvp.Key, kvp.Value)
        );

        return logEntry with { Properties = sanitizedProperties };
    }

    /// <summary>
    /// 민감한 데이터 값 정제
    /// </summary>
    private object SanitizeValue(string key, object value)
    {
        var sensitiveKeys = new[] { "password", "token", "secret", "key", "auth", "credential" };
        
        if (sensitiveKeys.Any(sk => key.Contains(sk, StringComparison.OrdinalIgnoreCase)))
        {
            return "***REDACTED***";
        }

        return value;
    }

    /// <summary>
    /// 사용자명 정제
    /// </summary>
    private static string SanitizeUsername(string username)
    {
        return string.IsNullOrWhiteSpace(username) ? "Unknown" : username.Trim();
    }

    /// <summary>
    /// User Agent 정제
    /// </summary>
    private static string SanitizeUserAgent(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return "Unknown";

        // User Agent에서 너무 긴 정보는 잘라내기
        return userAgent.Length > 200 ? userAgent[..200] + "..." : userAgent;
    }

    /// <summary>
    /// 경로 정제
    /// </summary>
    private static string SanitizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return "/";

        // 경로에서 민감한 정보 제거 (예: 토큰, 키 등)
        return path.Contains("token", StringComparison.OrdinalIgnoreCase) 
            ? System.Text.RegularExpressions.Regex.Replace(path, @"token=[^&]*", "token=***")
            : path;
    }

    /// <summary>
    /// 리소스 정리
    /// </summary>
    public void Dispose()
    {
        _flushTimer?.Dispose();
        
        // 남은 로그들 플러시
        FlushLogs(null);
    }
}
