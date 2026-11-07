using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Notifications.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class ReceivedNotificationRepository(AppDbContext context) : BaseRepository<ReceivedNotification>(context), IReceivedNotificationRepository
{
    public Task<List<ReceivedNotification>> FindByRecipientUserIdAndTargetBusinessIdAndReadAtAsync(string recipientUserId, string targetBusinessId, DateTimeOffset? readAt, CancellationToken cancellationToken = default) => Context.Set<ReceivedNotification>().Where(entity => entity.RecipientUserId == recipientUserId && entity.TargetBusinessId == targetBusinessId && entity.ReadAt == readAt).ToListAsync(cancellationToken);
    public Task<List<ReceivedNotification>> FindByRecipientUserIdAndTargetBusinessIdAsync(string recipientUserId, string targetBusinessId, CancellationToken cancellationToken = default) => Context.Set<ReceivedNotification>().Where(entity => entity.RecipientUserId == recipientUserId && entity.TargetBusinessId == targetBusinessId).ToListAsync(cancellationToken);
}
