using InstAlertPlatform.API.Business.Domain.Model.ValueObjects;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;
using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
public sealed class DevelopmentDataSeeder(AppDbContext db, IWebHostEnvironment environment, TimeProvider clock)
{
    public async Task Seed(CancellationToken token)
    {
        if (!environment.IsDevelopment())
            throw new InvalidOperationException("Synthetic fixtures are restricted to Development.");
        if (!await db.Set<Plan>().AnyAsync(p => p.Id == "essential", token))
            throw new InvalidOperationException("Apply database migrations before loading development fixtures.");
        await using var transaction = await db.Database.BeginTransactionAsync(token);
        if (!await db.Set<UserProfile>().AnyAsync(p => p.Id == "synthetic-admin", token))
            db.Add(new UserProfile("synthetic-admin", "Synthetic", "Administrator", "00000001", null));
        if (!await db.Set<UserProfile>().AnyAsync(p => p.Id == "synthetic-employee", token))
            db.Add(new UserProfile("synthetic-employee", "Synthetic", "Employee", "00000002", null));
        var business = await db.Set<BusinessProfile>().SingleOrDefaultAsync(b => b.TaxId == new TaxIdentifier("20000000001"), token);
        if (business is null)
        {
            business = new BusinessProfile("synthetic-admin", "20000000001", "Synthetic business", "Synthetic demonstration address", -12.0464, -77.0428);
            db.Add(business);
        }
        else if (business.OwnerUserId != "synthetic-admin")
            throw new InvalidOperationException("The development business belongs to a different owner.");

        await db.SaveChangesAsync(token);
        if (!await db.Set<BusinessMember>().AnyAsync(m => m.BusinessId == business.Id && m.UserId == "synthetic-employee", token))
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
