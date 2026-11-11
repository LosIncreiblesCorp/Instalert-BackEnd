using InstAlertPlatform.API.Business.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;
using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
public sealed class DevelopmentDataSeeder(AppDbContext db, IWebHostEnvironment environment, TimeProvider clock, IConfiguration configuration)
{
    public async Task Seed(CancellationToken token)
    {
        if (!environment.IsDevelopment() && !configuration.GetValue<bool>("Demo:Enabled"))
            throw new InvalidOperationException("Synthetic fixtures require Development or explicitly enabled demo mode.");
        if (!await db.Set<Plan>().AnyAsync(p => p.Id == "essential", token))
            throw new InvalidOperationException("Apply database migrations before loading development fixtures.");
        await using var transaction = await db.Database.BeginTransactionAsync(token);
        if (!await db.Set<UserProfile>().AnyAsync(p => p.Id == "synthetic-admin", token))
            db.Add(new UserProfile("synthetic-admin", "Synthetic", "Administrator", "00000001", null));
        if (!await db.Set<UserProfile>().AnyAsync(p => p.Id == "synthetic-employee", token))
            db.Add(new UserProfile("synthetic-employee", "Synthetic", "Employee", "00000002", null));
        var owned = await db.Set<BusinessProfile>().Where(b => b.OwnerUserId == "synthetic-admin").ToListAsync(token);
        if (owned.Count > 1)
            throw new InvalidOperationException("The demo administrator must resolve to a single commerce. Existing records were preserved.");
        var business = owned.SingleOrDefault();
        if (business is null)
        {
            if (await db.Set<BusinessProfile>().AnyAsync(b => b.TaxId == new TaxIdentifier("20000000001"), token))
                throw new InvalidOperationException("The demo tax identifier belongs to another owner. Existing records were preserved.");
            business = new BusinessProfile("synthetic-admin", "20000000001", "Synthetic business", "Synthetic demonstration address", -12.0464, -77.0428);
            db.Add(business);
        }

        await db.SaveChangesAsync(token);
        var memberships = await db.Set<BusinessMember>().Where(m => m.UserId == "synthetic-employee").ToListAsync(token);
        if (memberships.Count > 1 || memberships.Any(m => m.BusinessId != business.Id))
            throw new InvalidOperationException("The demo employee must belong to the demo administrator's commerce. Existing memberships were preserved.");
        if (memberships.Count == 0)
        {
            var member = new BusinessMember(business.Id.ToString(), "synthetic-employee", "Synthetic Employee", "employee@example.invalid");
            db.Add(member);
        }

        var businessId = business.Id.ToString();
        if (!await db.Set<Subscription>().AnyAsync(s => s.BusinessId == businessId, token))
        {
            var now = clock.GetUtcNow();
            var subscription = new Subscription(businessId, "essential", "synthetic-development-" + businessId);
            subscription.Confirm("essential", now.AddMonths(1), now);
            db.Add(subscription);
        }
        await db.SaveChangesAsync(token);
        await transaction.CommitAsync(token);
    }
}
