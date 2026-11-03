namespace InstAlertPlatform.API.Shared.Domain.Model;
public sealed class DomainException(string code, string field = "") : Exception(code)
{
    public string Code { get; } = code;
    public string Field { get; } = field;
}
