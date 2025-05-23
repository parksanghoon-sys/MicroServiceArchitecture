namespace ECommerce.Shared.Authentication;

public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;   
    public string SecurityKey { get; set; } = string.Empty;
}
