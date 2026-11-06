using InstAlertPlatform.API.Alert.Domain.Model.Aggregates;
using InstAlertPlatform.API.Alert.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Alert.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class AlertPreferencesRepository(AppDbContext context) : BaseRepository<AlertPreferences>(context), IAlertPreferencesRepository
{
}
