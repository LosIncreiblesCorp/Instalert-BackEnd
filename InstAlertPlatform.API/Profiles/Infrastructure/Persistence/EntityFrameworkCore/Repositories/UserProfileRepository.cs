using InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;
using InstAlertPlatform.API.Profiles.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class UserProfileRepository(AppDbContext context) : BaseRepository<UserProfile>(context), IUserProfileRepository
{
}
