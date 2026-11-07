using InstAlertPlatform.API.Dashboard.Application.Errors;
using InstAlertPlatform.API.Dashboard.Application.Services;
using InstAlertPlatform.API.Dashboard.Domain.Model.Commands;
using InstAlertPlatform.API.Shared.Application.Patterns;

namespace InstAlertPlatform.API.Dashboard.Application.Services;
public interface IReportCommandService
{
    Task<Result<ReportSnapshot, EGenerateReportError>> Handle(GenerateReportCommand command, CancellationToken cancellationToken = default);
}
