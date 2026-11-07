using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Notifications.Domain.Repositories;
public interface IReceivedNotificationRepository : IBaseRepository<ReceivedNotification>
{
    Task<List<ReceivedNotification>> FindByRecipientUserIdAndTargetBusinessIdAndReadAtAsync(string recipientUserId, string targetBusinessId, DateTimeOffset? readAt, CancellationToken cancellationToken = default);
    Task<List<ReceivedNotification>> FindByRecipientUserIdAndTargetBusinessIdAsync(string recipientUserId, string targetBusinessId, CancellationToken cancellationToken = default);
}
