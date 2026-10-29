using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Business.Application.Services;

public interface IBusinessProfileCommandService
{
    Task<Result<BusinessProfile, ECreateBusinessProfileError>> Handle(CreateBusinessProfileCommand command,
        CancellationToken cancellationToken = default);

    Task<Result<BusinessProfile, EUpdateBusinessProfileError>> Handle(UpdateBusinessProfileCommand command,
        CancellationToken cancellationToken = default);
}
