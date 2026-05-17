using GlobalLogisticsManagementUI.Models;

namespace GlobalLogisticsManagementUI.Services
{
    public interface IClientService
    {
        Task<IEnumerable<ClientViewModel>> GetAllAsync();
        Task<ClientViewModel?> GetByIdAsync(int id);
        Task<ClientViewModel> CreateAsync(ClientCreateViewModel model);
        Task UpdateAsync(int id, ClientViewModel model);
        Task DeleteAsync(int id);
    }
}
