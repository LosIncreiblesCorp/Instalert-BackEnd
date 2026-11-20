namespace InstAlertPlatform.API.Iam.Infrastructure.Tokens.Jwt.Configuration;

public sealed class TokenSettings
{
    public string Secret { get; set; } = "";
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public int AccessTokenMinutes { get; set; } = 10080;
}
