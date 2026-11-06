using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Contacts.Application.Errors;
using InstAlertPlatform.API.Contacts.Application.Services;
using InstAlertPlatform.API.Contacts.Domain.Model.Aggregates;
using InstAlertPlatform.API.Contacts.Domain.Model.Commands;
using InstAlertPlatform.API.Contacts.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Contacts.Application.Internal.CommandServices;
public sealed class EmergencyContactCommandService(IEmergencyContactRepository contacts, ICurrentActor actor, IUnitOfWork work, IBusinessProfileQueryService business) : IEmergencyContactCommandService
{
    public async Task<Result<ContactSnapshot, ECreateEmergencyContactError>> Handle(CreateEmergencyContactCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.Require(await business.Handle(new GetOperationalMembershipQuery(actor.UserId), cancellationToken), "forbidden");
            var contact = new EmergencyContact(actor.UserId, command.FullName, command.Relationship, command.Phone, command.Email, command.Notes);
            await contacts.AddAsync(contact, cancellationToken);
            await work.CompleteAsync(cancellationToken);
            return new Result<ContactSnapshot, ECreateEmergencyContactError>.Success(ToContactSnapshot(contact));
        }
        catch (DomainException error)
        {
            return new Result<ContactSnapshot, ECreateEmergencyContactError>.Failure(error.Code switch
            {
                "validation" => ECreateEmergencyContactError.InvalidInput,
                "unauthenticated" => ECreateEmergencyContactError.Unauthenticated,
                "forbidden" => ECreateEmergencyContactError.Forbidden,
                "not_found" => ECreateEmergencyContactError.NotFound,
                "conflict" => ECreateEmergencyContactError.Conflict,
                "concurrency" => ECreateEmergencyContactError.ConcurrencyConflict,
                "unexpected" => ECreateEmergencyContactError.UnexpectedError,
                _ => ECreateEmergencyContactError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<ContactSnapshot, EUpdateEmergencyContactError>> Handle(UpdateEmergencyContactCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var contact = await Own(id, cancellationToken);
            contact.Update(command.FullName, command.Relationship, command.Phone, command.Email, command.Notes);
            await work.CompleteAsync(cancellationToken);
            return new Result<ContactSnapshot, EUpdateEmergencyContactError>.Success(ToContactSnapshot(contact));
        }
        catch (DomainException error)
        {
            return new Result<ContactSnapshot, EUpdateEmergencyContactError>.Failure(error.Code switch
            {
                "validation" => EUpdateEmergencyContactError.InvalidInput,
                "unauthenticated" => EUpdateEmergencyContactError.Unauthenticated,
                "forbidden" => EUpdateEmergencyContactError.Forbidden,
                "not_found" => EUpdateEmergencyContactError.NotFound,
                "conflict" => EUpdateEmergencyContactError.Conflict,
                "concurrency" => EUpdateEmergencyContactError.ConcurrencyConflict,
                "unexpected" => EUpdateEmergencyContactError.UnexpectedError,
                _ => EUpdateEmergencyContactError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<bool, EDeleteEmergencyContactError>> Handle(DeleteEmergencyContactCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var contact = await Own(id, cancellationToken);
            contacts.Remove(contact);
            await work.CompleteAsync(cancellationToken);
            return new Result<bool, EDeleteEmergencyContactError>.Success(true);
        }
        catch (DomainException error)
        {
            return new Result<bool, EDeleteEmergencyContactError>.Failure(error.Code switch
            {
                "validation" => EDeleteEmergencyContactError.InvalidInput,
                "unauthenticated" => EDeleteEmergencyContactError.Unauthenticated,
                "forbidden" => EDeleteEmergencyContactError.Forbidden,
                "not_found" => EDeleteEmergencyContactError.NotFound,
                "conflict" => EDeleteEmergencyContactError.Conflict,
                "concurrency" => EDeleteEmergencyContactError.ConcurrencyConflict,
                "unexpected" => EDeleteEmergencyContactError.UnexpectedError,
                _ => EDeleteEmergencyContactError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
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
