using System.Net.Http.Json;
using GlobalLogisticsManagementUI.Models;

namespace GlobalLogisticsManagementUI.Services
{
    public class ClientService : IClientService
    {
        private readonly HttpClient _httpClient;

        public ClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ClientViewModel>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("api/Client");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ClientViewModel>>()
                ?? Enumerable.Empty<ClientViewModel>();
        }

        public async Task<ClientViewModel?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ClientViewModel>($"api/Client/{id}");
        }

        public async Task<ClientViewModel> CreateAsync(ClientCreateViewModel model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Client", model);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ClientViewModel>()
                ?? throw new Exception("Failed to create client.");
        }

        public async Task UpdateAsync(int id, ClientViewModel model)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Client/{id}", model);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Client/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
