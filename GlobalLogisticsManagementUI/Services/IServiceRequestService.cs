using GlobalLogisticsManagementUI.Models;

namespace GlobalLogisticsManagementUI.Services
{
    public interface IServiceRequestService
    {
        Task<IEnumerable<ServiceRequestViewModel>> GetAllAsync();
        Task<ServiceRequestViewModel?> GetByIdAsync(int id);
        Task<ServiceRequestViewModel> CreateAsync(ServiceRequestCreateViewModel model);
        Task UpdateAsync(int id, ServiceRequestViewModel model);
        Task DeleteAsync(int id);
    }
}
