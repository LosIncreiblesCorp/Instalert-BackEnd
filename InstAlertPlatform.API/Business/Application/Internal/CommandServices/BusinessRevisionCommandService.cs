using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Business.Application.Internal.CommandServices;
public sealed class BusinessRevisionCommandService(IBusinessProfileRepository businesses) : IBusinessRevisionCommandService
{
    public async Task<Result<bool, ETouchBusinessSeatsError>> Handle(TouchBusinessSeatsCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var business = await businesses.FindByIdAsync(command.BusinessId, cancellationToken) ?? throw new DomainException("not_found");
            business.TouchSeats();
            return new Result<bool, ETouchBusinessSeatsError>.Success(true);
        }
        catch (DomainException error)
        {
            return new Result<bool, ETouchBusinessSeatsError>.Failure(error.Code switch
            {
                "not_found" => ETouchBusinessSeatsError.NotFound,
                "unexpected" => ETouchBusinessSeatsError.UnexpectedError,
                _ => ETouchBusinessSeatsError.UnexpectedError
            })
            {
                Field = error.Field
            };
        }
    }
}
