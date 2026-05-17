using GlobalLogisticsManagementUI.Models;

namespace GlobalLogisticsManagementUI.Services
{
    public interface IContractService
    {
        Task<IEnumerable<ContractViewModel>> GetAllAsync();
        Task<ContractViewModel?> GetByIdAsync(int id);
        Task<ContractViewModel> CreateAsync(ContractCreateViewModel model);
        Task UpdateAsync(int id, ContractViewModel model);
        Task DeleteAsync(int id);
        Task UploadAgreementAsync(int id, Stream fileStream, string fileName);
        Task<(Stream Content, string FileName)?> DownloadAgreementAsync(int id);
    }
}
