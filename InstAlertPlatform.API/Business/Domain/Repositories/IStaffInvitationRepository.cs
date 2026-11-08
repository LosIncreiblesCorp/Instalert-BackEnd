using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Business.Domain.Repositories;
public interface IStaffInvitationRepository : IBaseRepository<StaffInvitation>
{
    Task<List<StaffInvitation>> FindPendingByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<List<StaffInvitation>> FindByBusinessIdAndEmailAndStatusAsync(string businessId, string email, string status, CancellationToken cancellationToken = default);
    Task<List<StaffInvitation>> FindByBusinessIdAndNotIdAndEmailAndStatusAsync(string businessId, string id, string email, string status, CancellationToken cancellationToken = default);
    Task<List<StaffInvitation>> FindByBusinessIdAndStatusAsync(string businessId, string status, CancellationToken cancellationToken = default);
    Task<List<StaffInvitation>> FindByBusinessIdAsync(string businessId, CancellationToken cancellationToken = default);
}
