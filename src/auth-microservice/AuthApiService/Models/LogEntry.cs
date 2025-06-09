namespace AuthApiService.Models;

/// <summary>
/// 로그 엔트리를 나타내는 레코드
/// </summary>
/// <param name="Id">로그 ID</param>
/// <param name="Level">로그 레벨</param>
/// <param name="Message">로그 메시지</param>
/// <param name="Timestamp">로그 시간</param>
/// <param name="UserId">사용자 ID (선택적)</param>
/// <param name="Category">로그 카테고리</param>
/// <param name="Properties">추가 속성</param>
public record LogEntry(
    string Id,
    LogLevel Level,
    string Message,
    DateTime Timestamp,
    int? UserId,
    string Category,
    Dictionary<string, object>? Properties = null
)
{
    /// <summary>
    /// 새 로그 엔트리 생성을 위한 정적 팩토리 메서드
    /// </summary>
    /// <param name="level">로그 레벨</param>
    /// <param name="message">메시지</param>
    /// <param name="category">카테고리</param>
    /// <param name="userId">사용자 ID</param>
    /// <param name="properties">추가 속성</param>
    /// <returns>새로운 LogEntry 인스턴스</returns>
    public static LogEntry Create(
        LogLevel level,
        string message,
        string category,
        int? userId = null,
        Dictionary<string, object>? properties = null)
        => new(Guid.NewGuid().ToString(), level, message, DateTime.UtcNow, userId, category, properties);
}
