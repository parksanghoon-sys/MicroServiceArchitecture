using AuthApiService.Interfaces;
using System.Diagnostics;

namespace AuthApiService.Middleware;

/// <summary>
/// API 요청 로깅 미들웨어
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggingService _loggingService;

    public RequestLoggingMiddleware(RequestDelegate next, ILoggingService loggingService)
    {
        _next = next;
        _loggingService = loggingService;
    }

    /// <summary>
    /// 미들웨어 실행
    /// </summary>
    /// <param name="context">HTTP 컨텍스트</param>
    /// <returns>작업 완료</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        // 요청 정보 수집
        var request = context.Request;
        var method = request.Method;
        var path = request.Path.Value ?? "/";
        var queryString = request.QueryString.HasValue ? request.QueryString.Value : "";
        var userAgent = request.Headers["User-Agent"].FirstOrDefault();
        var clientIp = GetClientIPAddress(context);
        var userId = GetUserId(context);

        // 요청 시작 로그
        await _loggingService.LogInformationAsync(
            $"API 요청 시작: {method} {path}",
            "RequestLogging",
            userId,
            new Dictionary<string, object>
            {
                ["RequestId"] = requestId,
                ["Method"] = method,
                ["Path"] = path,
                ["QueryString"] = queryString,
                ["UserAgent"] = userAgent ?? "Unknown",
                ["ClientIP"] = clientIp,
                ["Timestamp"] = DateTime.UtcNow,
                ["Phase"] = "Start"
            });

        // 요청 ID를 헤더에 추가
        context.Response.Headers.Add("X-Request-ID", requestId);

        try
        {
            // 다음 미들웨어 실행
            await _next(context);
        }
        catch (Exception ex)
        {
            // 예외 발생 시 로깅
            await _loggingService.LogErrorAsync(
                $"API 요청 처리 중 예외 발생: {method} {path}",
                ex,
                "RequestLogging",
                userId,
                new Dictionary<string, object>
                {
                    ["RequestId"] = requestId,
                    ["Method"] = method,
                    ["Path"] = path,
                    ["ClientIP"] = clientIp,
                    ["Duration"] = stopwatch.ElapsedMilliseconds,
                    ["Phase"] = "Exception"
                });

            throw; // 예외를 다시 던져서 상위에서 처리하도록 함
        }
        finally
        {
            stopwatch.Stop();

            // 응답 완료 로그
            var statusCode = context.Response.StatusCode;
            var duration = stopwatch.Elapsed;

            await _loggingService.LogApiRequestAsync(
                method,
                path,
                statusCode,
                duration,
                userId,
                clientIp,
                userAgent);

            // 상세 응답 로그
            await _loggingService.LogInformationAsync(
                $"API 요청 완료: {method} {path} - {statusCode} ({duration.TotalMilliseconds:F2}ms)",
                "RequestLogging",
                userId,
                new Dictionary<string, object>
                {
                    ["RequestId"] = requestId,
                    ["Method"] = method,
                    ["Path"] = path,
                    ["StatusCode"] = statusCode,
                    ["Duration"] = duration.TotalMilliseconds,
                    ["DurationString"] = $"{duration.TotalMilliseconds:F2}ms",
                    ["ClientIP"] = clientIp,
                    ["Phase"] = "Complete",
                    ["Success"] = statusCode < 400
                });
        }
    }

    /// <summary>
    /// 클라이언트 IP 주소 가져오기
    /// </summary>
    /// <param name="context">HTTP 컨텍스트</param>
    /// <returns>IP 주소</returns>
    private static string GetClientIPAddress(HttpContext context)
    {
        // X-Forwarded-For 헤더 확인 (로드 밸런서나 프록시 뒤에 있는 경우)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (ips.Length > 0)
            {
                return ips[0].Trim();
            }
        }

        // X-Real-IP 헤더 확인
        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(realIp))
        {
            return realIp;
        }

        // 직접 연결된 클라이언트 IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    /// <summary>
    /// 현재 사용자 ID 가져오기
    /// </summary>
    /// <param name="context">HTTP 컨텍스트</param>
    /// <returns>사용자 ID</returns>
    private static int? GetUserId(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = context.User.FindFirst("user_id") ?? 
                             context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
        }

        return null;
    }
}

/// <summary>
/// 요청 로깅 미들웨어 확장 메서드
/// </summary>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// 요청 로깅 미들웨어 추가
    /// </summary>
    /// <param name="builder">애플리케이션 빌더</param>
    /// <returns>애플리케이션 빌더</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}
