using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Business.Application.Internal.CommandServices;
public sealed class BusinessProfileCommandService(IBusinessProfileRepository businesses, IBusinessProfileQueryService access, ICurrentActor actor, IUnitOfWork work) : IBusinessProfileCommandService
{
    public async Task<Result<BusinessSnapshot, ECreateBusinessError>> Handle(CreateBusinessCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            Guard.Require(actor.IsAdministrator, "forbidden");
            var details = command;
            Guard.Coordinates(details.Latitude, details.Longitude, true);
            var businessProfile = new BusinessProfile(actor.UserId, details.TaxId, details.Name, details.Address, details.Latitude!.Value, details.Longitude!.Value);
            Guard.Require((await businesses.FindByTaxIdAsync(businessProfile.TaxId, cancellationToken)).Count == 0, "conflict");
            await businesses.AddAsync(businessProfile, cancellationToken);
            await work.CompleteAsync(cancellationToken);
            return new Result<BusinessSnapshot, ECreateBusinessError>.Success(ToBusinessSnapshot(businessProfile));
        }
        catch (ArgumentException)
        {
            return new Result<BusinessSnapshot, ECreateBusinessError>.Failure(ECreateBusinessError.InvalidInput);
        }
        catch (DomainException error)
        {
            return new Result<BusinessSnapshot, ECreateBusinessError>.Failure(error.Code switch
            {
                "validation" => ECreateBusinessError.InvalidInput,
                "unauthenticated" => ECreateBusinessError.Unauthenticated,
                "forbidden" => ECreateBusinessError.Forbidden,
                "not_found" => ECreateBusinessError.NotFound,
                "conflict" => ECreateBusinessError.Conflict,
                "concurrency" => ECreateBusinessError.ConcurrencyConflict,
                "capacity" => ECreateBusinessError.CapacityExceeded,
                "transition" => ECreateBusinessError.InvalidTransition,
                "provider_unavailable" => ECreateBusinessError.ProviderUnavailable,
                "identity_unavailable" => ECreateBusinessError.IdentityUnavailable,
                "unexpected" => ECreateBusinessError.UnexpectedError,
                _ => ECreateBusinessError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    public async Task<Result<BusinessSnapshot, EUpdateBusinessError>> Handle(UpdateBusinessCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var id = command.Id;
            var details = command;
            await access.Handle(new RequireBusinessAccessQuery(id, actor.UserId, true), cancellationToken);
            var businessProfile = (await businesses.FindByIdAsync(id, cancellationToken))!;
            Guard.Coordinates(details.Latitude, details.Longitude, true);
            businessProfile.Update(details.TaxId ?? businessProfile.TaxId.Value, details.Name, details.Address, details.Latitude!.Value, details.Longitude!.Value);
            await work.CompleteAsync(cancellationToken);
            return new Result<BusinessSnapshot, EUpdateBusinessError>.Success(ToBusinessSnapshot(businessProfile));
        }
        catch (ArgumentException)
        {
            return new Result<BusinessSnapshot, EUpdateBusinessError>.Failure(EUpdateBusinessError.InvalidInput);
        }
        catch (DomainException error)
        {
            return new Result<BusinessSnapshot, EUpdateBusinessError>.Failure(error.Code switch
            {
                "validation" => EUpdateBusinessError.InvalidInput,
                "unauthenticated" => EUpdateBusinessError.Unauthenticated,
                "forbidden" => EUpdateBusinessError.Forbidden,
                "not_found" => EUpdateBusinessError.NotFound,
                "conflict" => EUpdateBusinessError.Conflict,
                "concurrency" => EUpdateBusinessError.ConcurrencyConflict,
                "capacity" => EUpdateBusinessError.CapacityExceeded,
                "transition" => EUpdateBusinessError.InvalidTransition,
                "provider_unavailable" => EUpdateBusinessError.ProviderUnavailable,
                "identity_unavailable" => EUpdateBusinessError.IdentityUnavailable,
                "unexpected" => EUpdateBusinessError.UnexpectedError,
                _ => EUpdateBusinessError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }

    private static BusinessSnapshot ToBusinessSnapshot(BusinessProfile b) => new(b.Id.ToString(), b.OwnerUserId, b.TaxId.Value, b.Name, b.Address, b.Latitude, b.Longitude, b.CreatedAt, b.UpdatedAt);
}
