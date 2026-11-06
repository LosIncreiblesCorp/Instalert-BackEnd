using InstAlertPlatform.API.Payments.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Payments.Domain.Repositories;
public interface ICheckoutIntentRepository : IBaseRepository<CheckoutIntent>
{
}
