using InstAlertPlatform.API.Iam.Domain.Model.Aggregates;
using InstAlertPlatform.API.Iam.Application.Services;

namespace InstAlertPlatform.API.Iam.Application.Internal.OutboundServices;
public interface ITokenService
{
    AuthenticationSnapshot Create(User user);
}
