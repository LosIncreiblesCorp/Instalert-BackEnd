namespace InstAlertPlatform.API.Shared.Application.Patterns;
public interface IIntegrationEventHandler
{
    string EventType { get; }

    Task Handle(string payload, CancellationToken token);
}
