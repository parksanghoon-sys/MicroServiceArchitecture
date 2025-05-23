namespace ECommerce.Shared.Authentication;
public class AuthenticationOptions
{
    public const string AuthenticationSectionName = "Authentication";
    public string AuthMicroserviceBaseAddress { get; set; } = string.Empty;
    public JwtOptions? JwtOptions { get; set; }
}