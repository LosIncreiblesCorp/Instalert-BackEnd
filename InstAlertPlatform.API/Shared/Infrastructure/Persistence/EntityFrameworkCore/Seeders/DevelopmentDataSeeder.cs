using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using InstAlertPlatform.API.Profiles.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Seeders;

public static class DevelopmentDataSeeder
{
    private const string BusinessTaxId = "20987654321";
    private const string EssentialPlanId = "essential";
    private const string SyntheticSubscriptionId = "synthetic-development-instalert-base-essential";
    private const string SyntheticAdministratorDni = "00000001";
    private const string SyntheticEmployeeDni = "00000002";

    public static async Task SeedAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        if (!environment.IsDevelopment() || !configuration.GetValue<bool>("DevelopmentData:SeedOnStartup"))
            return;

        var allowedUserIds = configuration.GetSection("DevelopmentActors:UserIds").Get<string[]>() ?? [];
        var administratorIds = configuration.GetSection("DevelopmentActors:AdministratorUserIds").Get<string[]>() ?? [];
        var administratorId = administratorIds.FirstOrDefault(id => allowedUserIds.Contains(id, StringComparer.Ordinal));
        var employeeId = allowedUserIds.FirstOrDefault(id => !administratorIds.Contains(id, StringComparer.Ordinal));

        if (string.IsNullOrWhiteSpace(administratorId) || string.IsNullOrWhiteSpace(employeeId))
            throw new InvalidOperationException("DevelopmentData seeding requires an allowed development administrator and employee.");

        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DevelopmentDataSeeder");

        if (!await context.Set<UserProfile>().AnyAsync(profile => profile.Id == administratorId, cancellationToken))
        {
            await context.Set<UserProfile>().AddAsync(
                new UserProfile(administratorId, "Synthetic", "Administrator", SyntheticAdministratorDni, null),
                cancellationToken);
        }

        if (!await context.Set<UserProfile>().AnyAsync(profile => profile.Id == employeeId, cancellationToken))
        {
            await context.Set<UserProfile>().AddAsync(
                new UserProfile(employeeId, "Synthetic", "Employee", SyntheticEmployeeDni, null),
                cancellationToken);
        }

        var business = (await context.Set<BusinessProfile>().ToListAsync(cancellationToken))
            .SingleOrDefault(item => item.TaxId.Value == BusinessTaxId);

        if (business is null)
        {
            business = new BusinessProfile(
                administratorId,
                BusinessTaxId,
                "InstAlert Demo Business (synthetic)",
                "Av. Demo 100, Lima, Peru",
                -12.0464,
                -77.0428);
            await context.Set<BusinessProfile>().AddAsync(business, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        else if (!string.Equals(business.OwnerUserId, administratorId, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("The synthetic development business tax ID is already owned by a different user.");
        }

        var businessId = business.Id.ToString(System.Globalization.CultureInfo.InvariantCulture);
        var memberExists = await context.Set<BusinessMember>()
            .AnyAsync(member => member.BusinessId == business.Id && member.UserId == employeeId, cancellationToken);
        if (!memberExists)
        {
            await context.Set<BusinessMember>().AddAsync(
                new BusinessMember(businessId, employeeId, "Synthetic Employee", "synthetic-employee@example.invalid"),
                cancellationToken);
        }

        var subscriptionExists = await context.Set<Subscription>()
            .AnyAsync(subscription => subscription.BusinessId == businessId, cancellationToken);
        if (!subscriptionExists)
        {
            var planExists = await context.Set<Plan>().AnyAsync(plan => plan.Id == EssentialPlanId, cancellationToken);
            if (!planExists)
                throw new InvalidOperationException("The Essential plan must exist before development data is seeded.");

            var now = DateTimeOffset.UtcNow;
            var subscription = new Subscription(businessId, EssentialPlanId, SyntheticSubscriptionId);
            subscription.Confirm(EssentialPlanId, now.AddDays(30), now);
            await context.Set<Subscription>().AddAsync(subscription, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
            "Development seed ensured business {BusinessId}, administrator {AdministratorId}, and employee {EmployeeId} with synthetic profiles.",
            businessId,
            administratorId,
            employeeId);
    }
}
