namespace InstAlertPlatform.API.Shared.Application.Internal.EventHandlers;
public interface IIntegrationEventHandler
{
    string EventType { get; }

    Task Handle(string payload, CancellationToken token);
}
