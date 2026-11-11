using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Payments.Infrastructure.Persistence.EntityFrameworkCore.Configuration.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyPaymentsConfiguration(this ModelBuilder builder)
    {
        builder.Entity<PayPalWebhookReceipt>().ToTable("paypal_webhook_receipts");
        builder.Entity<PayPalWebhookReceipt>().Property(x => x.EventType).HasMaxLength(100);
        builder.Entity<Plan>().ToTable("plans");
        builder.Entity<Plan>().Property(x => x.Name).HasMaxLength(100);
        builder.Entity<Plan>().Property(x => x.Price).HasPrecision(10, 2);
        builder.Entity<Plan>().Property(x => x.Currency).HasMaxLength(3);
        builder.Entity<Plan>().Property(x => x.BillingCycle).HasMaxLength(10);
        builder.Entity<Plan>().Property(x => x.ProviderPlanId).HasMaxLength(100);
        builder.Entity<Plan>().HasIndex(x => x.ProviderPlanId).IsUnique();
        builder.Entity<Plan>().HasData(new { Id = "essential", Version = "catalog-v1", CreatedAt = (DateTimeOffset? )null, UpdatedAt = (DateTimeOffset? )null, Name = "Essential", Price = 15m, Currency = "USD", BillingCycle = "monthly", MaxEmployees = 4, ProviderPlanId = "P-9G467950ML2745201NLOCKGI" }, new { Id = "professional", Version = "catalog-v1", CreatedAt = (DateTimeOffset? )null, UpdatedAt = (DateTimeOffset? )null, Name = "Professional", Price = 30m, Currency = "USD", BillingCycle = "monthly", MaxEmployees = 8, ProviderPlanId = "P-66N5798614080793BNLOCKNY" }, new { Id = "business", Version = "catalog-v1", CreatedAt = (DateTimeOffset? )null, UpdatedAt = (DateTimeOffset? )null, Name = "Business", Price = 45m, Currency = "USD", BillingCycle = "monthly", MaxEmployees = 15, ProviderPlanId = "P-8D911646UD924981MNLOCK6Q" });
        builder.Entity<Subscription>().ToTable("subscriptions");
        builder.Entity<Subscription>().Property(x => x.BusinessId).HasMaxLength(80);
        builder.Entity<Subscription>().Property(x => x.PlanId).HasMaxLength(80);
        builder.Entity<Subscription>().Property(x => x.ProviderSubscriptionId).HasMaxLength(100);
        builder.Entity<Subscription>().Property(x => x.Status).HasMaxLength(20);
        builder.Entity<Subscription>().HasIndex(x => x.BusinessId).IsUnique();
        builder.Entity<Subscription>().HasIndex(x => x.ProviderSubscriptionId).IsUnique();
        builder.Entity<Subscription>().HasIndex(x => x.PlanId).HasDatabaseName("i_x_subscriptions_plan_id");
        builder.Entity<Subscription>().HasOne<Plan>().WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("f_k_subscriptions_plans_plan_id");
        builder.Entity<CheckoutIntent>().ToTable("checkout_intents");
        builder.Entity<CheckoutIntent>().Property(x => x.BusinessId).HasMaxLength(80);
        builder.Entity<CheckoutIntent>().Property(x => x.PlanId).HasMaxLength(80);
        builder.Entity<CheckoutIntent>().HasIndex(x => new { x.BusinessId, x.Completed, x.ExpiresAt }).HasDatabaseName("i_x_checkout_intents_business_id_completed_expires_at");
        builder.Entity<CheckoutIntent>().HasIndex(x => x.PlanId).HasDatabaseName("i_x_checkout_intents_plan_id");
        builder.Entity<CheckoutIntent>().HasOne<Plan>().WithMany().HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict).HasConstraintName("f_k_checkout_intents_plans_plan_id");
        builder.Entity<PaymentTransaction>().ToTable("payment_transactions");
        builder.Entity<PaymentTransaction>().Property(x => x.BusinessId).HasMaxLength(80);
        builder.Entity<PaymentTransaction>().Property(x => x.ProviderTransactionId).HasMaxLength(100);
        builder.Entity<PaymentTransaction>().HasIndex(x => x.ProviderTransactionId).IsUnique();
        builder.Entity<PaymentTransaction>().HasIndex(x => new { x.BusinessId, x.PaidAt }).HasDatabaseName("i_x_payment_transactions_business_id_paid_at");
        builder.Entity<PaymentTransaction>().Property(x => x.Amount).HasPrecision(10, 2);
        builder.Entity<PaymentTransaction>().Property(x => x.Currency).HasMaxLength(3);
    }

    public static void ApplyPaymentsFinalConfiguration(this ModelBuilder builder)
    {
        builder.Entity<Plan>().Property(x => x.Id).HasMaxLength(80);
    }
}
