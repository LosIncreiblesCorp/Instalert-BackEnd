using InstAlertPlatform.API.Business.Application.Errors;
using InstAlertPlatform.API.Business.Application.Services;
using InstAlertPlatform.API.Business.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Business.Application.Services;
public interface IBusinessProfileCommandService
{
    Task<Result<BusinessSnapshot, ECreateBusinessError>> Handle(CreateBusinessCommand command, CancellationToken cancellationToken = default);
    Task<Result<BusinessSnapshot, EUpdateBusinessError>> Handle(UpdateBusinessCommand command, CancellationToken cancellationToken = default);
}
