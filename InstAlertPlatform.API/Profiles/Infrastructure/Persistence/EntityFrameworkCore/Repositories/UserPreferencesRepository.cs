using InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;
using InstAlertPlatform.API.Profiles.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace InstAlertPlatform.API.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public sealed class UserPreferencesRepository(AppDbContext context)
    : BaseRepository<UserPreferences>(context), IUserPreferencesRepository;
