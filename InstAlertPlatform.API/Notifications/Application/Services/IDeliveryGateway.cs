namespace InstAlertPlatform.API.Notifications.Application.Services;
public interface IDeliveryGateway
{
    bool Configured(string channel);
    Task Send(DeliveryEnvelope message, CancellationToken token);
}
