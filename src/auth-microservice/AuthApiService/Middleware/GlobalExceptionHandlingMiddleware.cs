using AuthApiService.Interfaces;
using AuthApiService.Models;
using System.Net;
using System.Text.Json;

namespace AuthApiService.Middleware;

/// <summary>
/// 글로벌 예외 처리 미들웨어
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandlingMiddleware(
        RequestDelegate next,        
        IWebHostEnvironment environment)
    {
        _next = next;        
        _environment = environment;
    }

    /// <summary>
    /// 미들웨어 실행
    /// </summary>
    /// <param name="context">HTTP 컨텍스트</param>
    /// <returns>작업 완료</returns>
    public async Task InvokeAsync(HttpContext context,ILoggingService logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, logger);
        }
    }

    /// <summary>
    /// 예외 처리
    /// </summary>
    /// <param name="context">HTTP 컨텍스트</param>
    /// <param name="exception">발생한 예외</param>
    /// <returns>작업 완료</returns>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception, ILoggingService logger)
    {
        var requestId = context.Response.Headers["X-Request-ID"].FirstOrDefault() ?? Guid.NewGuid().ToString();
        var userId = GetUserId(context);
        var clientIp = GetClientIPAddress(context);

        // 예외 로깅
        await logger.LogErrorAsync(
            $"처리되지 않은 예외 발생: {exception.Message}",
            exception,
            "GlobalExceptionHandler",
            userId,
            new Dictionary<string, object>
            {
                ["RequestId"] = requestId,
                ["Path"] = context.Request.Path.Value ?? "/",
                ["Method"] = context.Request.Method,
                ["ClientIP"] = clientIp,
                ["UserAgent"] = context.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown",
                ["ExceptionType"] = exception.GetType().Name,
                ["StackTrace"] = exception.StackTrace ?? ""
            });

        // HTTP 응답 설정
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = GetStatusCode(exception);

        // 응답 메시지 생성
        var response = CreateErrorResponse(exception, requestId);

        // JSON 직렬화 옵션
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var jsonResponse = JsonSerializer.Serialize(response, jsonOptions);
        await context.Response.WriteAsync(jsonResponse);
    }

    /// <summary>
    /// 예외 타입에 따른 HTTP 상태 코드 결정
    /// </summary>
    /// <param name="exception">예외</param>
    /// <returns>HTTP 상태 코드</returns>
    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ArgumentException => (int)HttpStatusCode.BadRequest,            
            InvalidOperationException => (int)HttpStatusCode.BadRequest,
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
            NotImplementedException => (int)HttpStatusCode.NotImplemented,
            TimeoutException => (int)HttpStatusCode.RequestTimeout,
            TaskCanceledException => (int)HttpStatusCode.RequestTimeout,
            OperationCanceledException => (int)HttpStatusCode.RequestTimeout,
            FileNotFoundException => (int)HttpStatusCode.NotFound,
            DirectoryNotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }

    /// <summary>
    /// 에러 응답 객체 생성
    /// </summary>
    /// <param name="exception">예외</param>
    /// <param name="requestId">요청 ID</param>
    /// <returns>에러 응답</returns>
    private ApiResponse<object> CreateErrorResponse(Exception exception, string requestId)
    {
        var statusCode = GetStatusCode(exception);
        var message = GetUserFriendlyMessage(exception, statusCode);
        var errors = new List<string>();

        // 개발 환경에서만 상세 에러 정보 포함
        if (_environment.IsDevelopment())
        {
            errors.Add($"Exception Type: {exception.GetType().Name}");
            errors.Add($"Message: {exception.Message}");
            
            if (exception.InnerException != null)
            {
                errors.Add($"Inner Exception: {exception.InnerException.Message}");
            }

            errors.Add($"Request ID: {requestId}");
        }
        else
        {
            errors.Add($"Request ID: {requestId}");
        }

        return ApiResponse<object>.ErrorResponse(message, errors);
    }

    /// <summary>
    /// 사용자 친화적인 에러 메시지 생성
    /// </summary>
    /// <param name="exception">예외</param>
    /// <param name="statusCode">HTTP 상태 코드</param>
    /// <returns>사용자 친화적인 메시지</returns>
    private static string GetUserFriendlyMessage(Exception exception, int statusCode)
    {
        return statusCode switch
        {
            400 => "요청이 올바르지 않습니다. 입력 값을 확인해주세요.",
            401 => "인증이 필요합니다. 로그인 후 다시 시도해주세요.",
            403 => "이 작업을 수행할 권한이 없습니다.",
            404 => "요청한 리소스를 찾을 수 없습니다.",
            408 => "요청 시간이 초과되었습니다. 다시 시도해주세요.",
            409 => "요청이 현재 서버 상태와 충돌합니다.",
            429 => "너무 많은 요청이 발생했습니다. 잠시 후 다시 시도해주세요.",
            500 => "서버 내부 오류가 발생했습니다. 잠시 후 다시 시도해주세요.",
            501 => "요청한 기능이 구현되지 않았습니다.",
            502 => "게이트웨이 오류가 발생했습니다.",
            503 => "서비스를 일시적으로 사용할 수 없습니다. 잠시 후 다시 시도해주세요.",
            _ => "알 수 없는 오류가 발생했습니다. 관리자에게 문의해주세요."
        };
    }

    /// <summary>
    /// 클라이언트 IP 주소 가져오기
    /// </summary>
    /// <param name="context">HTTP 컨텍스트</param>
    /// <returns>IP 주소</returns>
    private static string GetClientIPAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(realIp))
        {
            return realIp;
        }

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
/// 글로벌 예외 처리 미들웨어 확장 메서드
/// </summary>
public static class GlobalExceptionHandlingMiddlewareExtensions
{
    /// <summary>
    /// 글로벌 예외 처리 미들웨어 추가
    /// </summary>
    /// <param name="builder">애플리케이션 빌더</param>
    /// <returns>애플리케이션 빌더</returns>
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
}
