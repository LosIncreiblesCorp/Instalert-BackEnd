using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Business.Domain.Repositories;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Business.Application.Internal.CommandServices;

public sealed class BusinessRevisionCommandService(IBusinessProfileRepository businesses)
    : IBusinessRevisionCommandService
{
    public async Task<Result<bool, ETouchBusinessSeatsError>> Handle(TouchBusinessSeatsCommand command,
        CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(command.BusinessId, out var businessId) || businessId <= 0)
            return new Result<bool, ETouchBusinessSeatsError>.Failure(ETouchBusinessSeatsError.NotFound);

        var business = await businesses.FindByIdAsync(businessId, cancellationToken);
        if (business is null)
            return new Result<bool, ETouchBusinessSeatsError>.Failure(ETouchBusinessSeatsError.NotFound);

        business.TouchSeats();
        return new Result<bool, ETouchBusinessSeatsError>.Success(true);
    }
}
