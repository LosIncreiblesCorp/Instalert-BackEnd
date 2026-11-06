using InstAlertPlatform.API.Payments.Application.Services;
using InstAlertPlatform.API.Payments.Interfaces.Rest.Resources;

namespace InstAlertPlatform.API.Payments.Interfaces.Rest.Transform;
public static class PaymentResourceFromSnapshotAssembler
{
    public static PaymentResource ToResourceFromSnapshot(PaymentSnapshot snapshot) => new(snapshot.Id, snapshot.BusinessId, snapshot.Folio, snapshot.IssuedAt, snapshot.Amount, snapshot.Currency, snapshot.Status);
}
