using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Contacts.Application.Services;
using InstAlertPlatform.API.Contacts.Domain.Model.Aggregates;
using InstAlertPlatform.API.Contacts.Domain.Model.Commands;
using InstAlertPlatform.API.Contacts.Domain.Model.Queries;
using InstAlertPlatform.API.Contacts.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Contacts.Application.Internal.QueryServices;
public sealed class EmergencyContactQueryService(IEmergencyContactRepository contacts, ICurrentActor actor, IBusinessProfileQueryService business) : IEmergencyContactQueryService
{
    public async Task<IReadOnlyList<ContactSnapshot>> Handle(GetEmergencyContactsQuery query, CancellationToken cancellationToken = default)
    {
        var id = actor.UserId;
        Guard.Require(await business.Handle(new GetOperationalMembershipQuery(id), cancellationToken), "forbidden");
        return (await contacts.FindByEmployeeIdAsync(id, cancellationToken)).Select(ToContactSnapshot).ToList();
    }

    public async Task<ContactSnapshot> Handle(GetEmergencyContactByIdQuery query, CancellationToken cancellationToken = default)
    {
        var id = query.Id;
        return ToContactSnapshot(await Own(id, cancellationToken));
    }

    public async Task<IReadOnlyList<ContactRecipient>> Handle(GetEmergencyContactRecipientsQuery query, CancellationToken cancellationToken = default)
    {
        var employeeId = query.EmployeeId;
        return (await contacts.FindByEmployeeIdAndNotEmailAsync(employeeId, null, cancellationToken)).Select(contact => new ContactRecipient(contact.Id, contact.FullName, contact.Email!)).ToList();
    }

    private async Task<EmergencyContact> Own(string id, CancellationToken cancellationToken)
    {
        var contact = await contacts.FindByIdAsync(id, cancellationToken);
        Guard.Require(contact is not null && contact.EmployeeId == actor.UserId, "not_found");
        Guard.Require(await business.Handle(new GetOperationalMembershipQuery(actor.UserId), cancellationToken), "forbidden");
        return contact!;
    }

    private static ContactSnapshot ToContactSnapshot(EmergencyContact c) => new(c.Id, c.EmployeeId, c.FullName, c.Relationship, c.Phone, c.Email, c.Notes, c.Version);
}
