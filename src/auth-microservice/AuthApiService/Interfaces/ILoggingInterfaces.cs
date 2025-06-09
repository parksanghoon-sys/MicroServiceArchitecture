using AuthApiService.Models;

namespace AuthApiService.Interfaces;

/// <summary>
/// 로깅 서비스 인터페이스
/// </summary>
public interface ILoggingService
{
    /// <summary>
    /// 정보 로그 기록
    /// </summary>
    /// <param name="message">로그 메시지</param>
    /// <param name="category">카테고리</param>
    /// <param name="userId">사용자 ID</param>
    /// <param name="properties">추가 속성</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task LogInformationAsync(
        string message,
        string category = "General",
        int? userId = null,
        Dictionary<string, object>? properties = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 경고 로그 기록
    /// </summary>
    /// <param name="message">로그 메시지</param>
    /// <param name="category">카테고리</param>
    /// <param name="userId">사용자 ID</param>
    /// <param name="properties">추가 속성</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task LogWarningAsync(
        string message,
        string category = "General",
        int? userId = null,
        Dictionary<string, object>? properties = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 에러 로그 기록
    /// </summary>
    /// <param name="message">로그 메시지</param>
    /// <param name="exception">예외 정보</param>
    /// <param name="category">카테고리</param>
    /// <param name="userId">사용자 ID</param>
    /// <param name="properties">추가 속성</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task LogErrorAsync(
        string message,
        Exception? exception = null,
        string category = "Error",
        int? userId = null,
        Dictionary<string, object>? properties = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 디버그 로그 기록
    /// </summary>
    /// <param name="message">로그 메시지</param>
    /// <param name="category">카테고리</param>
    /// <param name="userId">사용자 ID</param>
    /// <param name="properties">추가 속성</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task LogDebugAsync(
        string message,
        string category = "Debug",
        int? userId = null,
        Dictionary<string, object>? properties = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 인증 관련 로그 기록
    /// </summary>
    /// <param name="action">인증 액션</param>
    /// <param name="username">사용자명</param>
    /// <param name="success">성공 여부</param>
    /// <param name="ipAddress">IP 주소</param>
    /// <param name="userAgent">User Agent</param>
    /// <param name="additionalInfo">추가 정보</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task LogAuthenticationAsync(
        string action,
        string username,
        bool success,
        string? ipAddress = null,
        string? userAgent = null,
        Dictionary<string, object>? additionalInfo = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// API 요청 로그 기록
    /// </summary>
    /// <param name="method">HTTP 메서드</param>
    /// <param name="path">요청 경로</param>
    /// <param name="statusCode">응답 상태 코드</param>
    /// <param name="duration">처리 시간</param>
    /// <param name="userId">사용자 ID</param>
    /// <param name="ipAddress">IP 주소</param>
    /// <param name="userAgent">User Agent</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task LogApiRequestAsync(
        string method,
        string path,
        int statusCode,
        TimeSpan duration,
        int? userId = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 로그 조회
    /// </summary>
    /// <param name="level">로그 레벨</param>
    /// <param name="category">카테고리</param>
    /// <param name="userId">사용자 ID</param>
    /// <param name="startDate">시작 날짜</param>
    /// <param name="endDate">종료 날짜</param>
    /// <param name="pageNumber">페이지 번호</param>
    /// <param name="pageSize">페이지 크기</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>로그 목록</returns>
    Task<IEnumerable<LogEntry>> GetLogsAsync(
        LogLevel? level = null,
        string? category = null,
        int? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 구조화된 로그 기록
    /// </summary>
    /// <param name="logEntry">로그 엔트리</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task LogAsync(LogEntry logEntry, CancellationToken cancellationToken = default);
}

/// <summary>
/// 성능 모니터링 서비스 인터페이스
/// </summary>
public interface IPerformanceMonitoringService
{
    /// <summary>
    /// 성능 메트릭 기록
    /// </summary>
    /// <param name="operationName">작업 이름</param>
    /// <param name="duration">소요 시간</param>
    /// <param name="success">성공 여부</param>
    /// <param name="additionalMetrics">추가 메트릭</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task RecordMetricAsync(
        string operationName,
        TimeSpan duration,
        bool success = true,
        Dictionary<string, object>? additionalMetrics = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 응답 시간 기록
    /// </summary>
    /// <param name="endpoint">엔드포인트</param>
    /// <param name="responseTime">응답 시간</param>
    /// <param name="statusCode">상태 코드</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task RecordResponseTimeAsync(
        string endpoint,
        TimeSpan responseTime,
        int statusCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 시스템 메트릭 기록
    /// </summary>
    /// <param name="metricName">메트릭 이름</param>
    /// <param name="value">값</param>
    /// <param name="unit">단위</param>
    /// <param name="tags">태그</param>
    /// <param name="cancellationToken">취소 토큰</param>
    /// <returns>작업 완료</returns>
    Task RecordSystemMetricAsync(
        string metricName,
        double value,
        string unit = "",
        Dictionary<string, string>? tags = null,
        CancellationToken cancellationToken = default);
}
