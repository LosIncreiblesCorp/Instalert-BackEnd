using InstAlertPlatform.API.Iam.Domain.Model.Aggregates;
using InstAlertPlatform.API.Iam.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public sealed class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public Task<User?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default) =>
        Context.Set<User>().SingleOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
}
