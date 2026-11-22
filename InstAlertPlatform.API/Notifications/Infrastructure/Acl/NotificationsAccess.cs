using InstAlertPlatform.API.Notifications.Application.Internal.OutboundServices;
using InstAlertPlatform.API.Iam.Application.QueryServices;
using InstAlertPlatform.API.Iam.Domain.Model.Queries;
using InstAlertPlatform.API.Iam.Interfaces.Acl;
using InstAlertPlatform.API.Business.Application.QueryServices;
using InstAlertPlatform.API.Business.Domain.Model.Queries;
using InstAlertPlatform.API.Payments.Application.QueryServices;
using InstAlertPlatform.API.Payments.Domain.Model.Queries;
using InstAlertPlatform.API.Shared.Domain.Model;

namespace InstAlertPlatform.API.Notifications.Infrastructure.Acl;
public sealed class NotificationsAccess(IUserQueryService users, IIamContextFacade identity,
    IBusinessProfileQueryService businesses, IPlanQueryService plans) : INotificationsAccess
{
    public async Task Require(CancellationToken token)
    {
        var session = await users.Handle(new GetCurrentUserQuery(), token);
        Guard.Require(session.HasApplicationAccess, "forbidden");
    }

    public async Task<bool> CanReceive(string userId, string businessId, CancellationToken token)
    {
        if (!await identity.HasCompletedAccount(userId, token)) return false;
        if (!(await plans.Handle(new GetPlanLimitsQuery(businessId), token)).SubscriptionActive) return false;
        try
        {
            await businesses.Handle(new RequireBusinessAccessQuery(businessId, userId, false), token);
            return true;
        }
        catch (DomainException error) when (error.Code is "not_found" or "forbidden") { return false; }
    }
}
