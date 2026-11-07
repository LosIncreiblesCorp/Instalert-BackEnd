using InstAlertPlatform.API.Notifications.Domain.Model.Aggregates;
using InstAlertPlatform.API.Notifications.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Notifications.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class NotificationPreferencesRepository(AppDbContext context) : BaseRepository<NotificationPreferences>(context), INotificationPreferencesRepository
{
}
