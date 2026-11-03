using InstAlertPlatform.API.Business.Domain.Model.Aggregates;
using InstAlertPlatform.API.Shared.Domain.Repositories;

namespace InstAlertPlatform.API.Business.Domain.Repositories;
public interface IBusinessMemberRepository : IBaseRepository<BusinessMember>
{
    Task<List<BusinessMember>> FindByBusinessIdAndEmailAsync(string businessId, string email, CancellationToken cancellationToken = default);
    Task<List<BusinessMember>> FindByBusinessIdAndStatusAsync(string businessId, string status, CancellationToken cancellationToken = default);
    Task<List<BusinessMember>> FindByBusinessIdAndUserIdAndStatusAsync(string businessId, string userId, string status, CancellationToken cancellationToken = default);
    Task<List<BusinessMember>> FindByBusinessIdAndUserIdAsync(string businessId, string userId, CancellationToken cancellationToken = default);
    Task<List<BusinessMember>> FindByBusinessIdAsync(string businessId, CancellationToken cancellationToken = default);
    Task<List<BusinessMember>> FindByUserIdAndStatusAsync(string userId, string status, CancellationToken cancellationToken = default);
    Task<List<BusinessMember>> FindByUserIdAsync(string userId, CancellationToken cancellationToken = default);
}
