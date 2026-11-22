namespace InstAlertPlatform.API.Notifications.Application.Internal.OutboundServices;
public interface IDeliveryGateway
{
    bool Configured(string channel);
    Task Send(DeliveryEnvelope message, CancellationToken token);
}
