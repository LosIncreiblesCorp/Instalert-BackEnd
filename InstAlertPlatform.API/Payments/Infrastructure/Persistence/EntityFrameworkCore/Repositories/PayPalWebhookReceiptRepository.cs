using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using InstAlertPlatform.API.Payments.Domain.Repositories;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using InstAlertPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Payments.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
public sealed class PayPalWebhookReceiptRepository(AppDbContext context) : BaseRepository<PayPalWebhookReceipt>(context), IPayPalWebhookReceiptRepository
{
}
