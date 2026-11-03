using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Business.Application.Services;

public interface IBusinessRevisionCommandService
{
    Task<Result<bool, ETouchBusinessSeatsError>> Handle(TouchBusinessSeatsCommand command,
        CancellationToken cancellationToken = default);
}
