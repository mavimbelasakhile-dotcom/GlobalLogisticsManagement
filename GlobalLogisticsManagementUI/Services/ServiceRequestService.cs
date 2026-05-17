using System.Net.Http.Json;
using GlobalLogisticsManagementUI.Models;

namespace GlobalLogisticsManagementUI.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly HttpClient _httpClient;

        public ServiceRequestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ServiceRequestViewModel>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("api/ServiceRequest");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ServiceRequestViewModel>>()
                ?? Enumerable.Empty<ServiceRequestViewModel>();
        }

        public async Task<ServiceRequestViewModel?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ServiceRequestViewModel>($"api/ServiceRequest/{id}");
        }

        public async Task<ServiceRequestViewModel> CreateAsync(ServiceRequestCreateViewModel model)
        {
            var payload = new
            {
                contractId = model.ContractId,
                description = model.Description,
                costUsd = model.CostUsd,
                status = model.Status
            };

            var response = await _httpClient.PostAsJsonAsync("api/ServiceRequest", payload);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Error: {error}");
            }

            return await response.Content.ReadFromJsonAsync<ServiceRequestViewModel>()
                ?? new ServiceRequestViewModel { ContractId = model.ContractId, Description = model.Description };
        }

        public async Task UpdateAsync(int id, ServiceRequestViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/ServiceRequest/{id}", model);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Update failed: {error}");
            }
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/ServiceRequest/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
