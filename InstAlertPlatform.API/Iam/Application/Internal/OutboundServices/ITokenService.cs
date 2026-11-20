using InstAlertPlatform.API.Iam.Application.Services.Snapshots;
using InstAlertPlatform.API.Iam.Domain.Model.Aggregates;
namespace InstAlertPlatform.API.Iam.Application.Internal.OutboundServices;
public interface ITokenService
{
    AuthenticationSnapshot Create(User user);
}
