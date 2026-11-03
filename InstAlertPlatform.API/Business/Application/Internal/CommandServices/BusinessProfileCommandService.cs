using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InstAlertPlatform.API.Business.Application.Internal.CommandServices;

public class BusinessProfileCommandService(IBusinessProfileRepository repository, IUnitOfWork unitOfWork,
    ILogger<BusinessProfileCommandService> logger) : IBusinessProfileCommandService
{
    public async Task<Result<BusinessProfile, ECreateBusinessProfileError>> Handle(
        CreateBusinessProfileCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var profile = new BusinessProfile(command);
            if (await repository.FindByTaxIdAsync(profile.TaxId, cancellationToken) is not null)
                return new Result<BusinessProfile, ECreateBusinessProfileError>.Failure(ECreateBusinessProfileError.DuplicateTaxIdentifier);

            await repository.AddAsync(profile, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<BusinessProfile, ECreateBusinessProfileError>.Success(profile);
        }
        catch (ArgumentException)
        {
            return new Result<BusinessProfile, ECreateBusinessProfileError>.Failure(ECreateBusinessProfileError.InvalidDetails);
        }
        catch (DbUpdateException error) when (IsDuplicateKeyViolation(error))
        {
            logger.LogWarning("A duplicate business tax identifier was rejected.");
            return new Result<BusinessProfile, ECreateBusinessProfileError>.Failure(ECreateBusinessProfileError.DuplicateTaxIdentifier);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception error)
        {
            logger.LogError(error, "Business profile creation failed.");
            return new Result<BusinessProfile, ECreateBusinessProfileError>.Failure(ECreateBusinessProfileError.UnexpectedError);
        }
    }

    public async Task<Result<BusinessProfile, EUpdateBusinessProfileError>> Handle(
        UpdateBusinessProfileCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var profile = await repository.FindByIdAsync(command.Id, cancellationToken);
            if (profile is null)
                return new Result<BusinessProfile, EUpdateBusinessProfileError>.Failure(EUpdateBusinessProfileError.NotFound);
            if (profile.OwnerUserId != command.OwnerUserId)
                return new Result<BusinessProfile, EUpdateBusinessProfileError>.Failure(EUpdateBusinessProfileError.Forbidden);
            if (command.TaxId is not null && command.TaxId != profile.TaxId)
                return new Result<BusinessProfile, EUpdateBusinessProfileError>.Failure(EUpdateBusinessProfileError.TaxIdentifierReadOnly);

            profile.Update(command);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<BusinessProfile, EUpdateBusinessProfileError>.Success(profile);
        }
        catch (ArgumentException)
        {
            return new Result<BusinessProfile, EUpdateBusinessProfileError>.Failure(EUpdateBusinessProfileError.InvalidDetails);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception error)
        {
            logger.LogError(error, "Business profile update failed.");
            return new Result<BusinessProfile, EUpdateBusinessProfileError>.Failure(EUpdateBusinessProfileError.UnexpectedError);
        }
    }

    private static bool IsDuplicateKeyViolation(DbUpdateException exception)
    {
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            if (current.GetType().Name == "MySqlException" &&
                current.GetType().GetProperty("Number")?.GetValue(current) is int number && number == 1062)
                return true;
        }
        return false;
    }
}
