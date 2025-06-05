namespace ECommerce.Shared.Authentication;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;   
    public string SecurityKey { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; } = 15; // 15분
    public int RefreshTokenExpirationDays { get; set; } = 7; // 7일
}
