using InstAlertPlatform.API.Contacts.Application.Services;
using InstAlertPlatform.API.Contacts.Domain.Model.Queries;

namespace InstAlertPlatform.API.Contacts.Application.Services;
public interface IEmergencyContactQueryService
{
    Task<IReadOnlyList<ContactSnapshot>> Handle(GetEmergencyContactsQuery query, CancellationToken cancellationToken = default);
    Task<ContactSnapshot> Handle(GetEmergencyContactByIdQuery query, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContactRecipient>> Handle(GetEmergencyContactRecipientsQuery query, CancellationToken cancellationToken = default);
}
