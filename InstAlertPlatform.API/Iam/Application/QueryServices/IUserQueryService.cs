using InstAlertPlatform.API.Iam.Application.Services;
using InstAlertPlatform.API.Iam.Domain.Model.Queries;
namespace InstAlertPlatform.API.Iam.Application.QueryServices;
public interface IUserQueryService
{
    Task<UserSnapshot> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken = default);
}
