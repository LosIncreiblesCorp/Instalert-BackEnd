namespace InstAlertPlatform.API.Shared.Domain.Model;
public abstract class AuditedEntity
{
    public string Id { get; protected set; } = Guid.NewGuid().ToString();
    public string Version { get; private set; } = Guid.NewGuid().ToString();

    public void RenewVersion() => Version = Guid.NewGuid().ToString();
}
