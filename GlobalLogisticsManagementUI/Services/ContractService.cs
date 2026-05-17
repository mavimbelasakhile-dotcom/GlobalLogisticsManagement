using System.Net.Http.Json;
using GlobalLogisticsManagementUI.Models;

namespace GlobalLogisticsManagementUI.Services
{
    public class ContractService : IContractService
    {
        private readonly HttpClient _httpClient;

        public ContractService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<ContractViewModel>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("api/Contract");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ContractViewModel>>()
                ?? Enumerable.Empty<ContractViewModel>();
        }

        public async Task<ContractViewModel?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<ContractViewModel>($"api/Contract/{id}");
        }

        public async Task<ContractViewModel> CreateAsync(ContractCreateViewModel model)
        {
            var payload = new
            {
                clientId = model.ClientId,
                startDate = model.StartDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                endDate = model.EndDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                status = model.Status,
                serviceLevel = model.ServiceLevel ?? "Standard"
            };

            var response = await _httpClient.PostAsJsonAsync("api/Contract", payload);

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Validation Error: {error}");
            }

            return new ContractViewModel
            {
                ClientId = model.ClientId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Status = model.Status,
                ServiceLevel = model.ServiceLevel
            };
        }

        public async Task UpdateAsync(int id, ContractViewModel model)
        {
            var payload = new
            {
                id = model.Id,
                clientId = model.ClientId,
                startDate = model.StartDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                endDate = model.EndDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                status = model.Status,
                serviceLevel = model.ServiceLevel
            };

            var response = await _httpClient.PutAsJsonAsync($"api/Contract/{id}", payload);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Update failed: {error}");
            }
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Contract/{id}");
            response.EnsureSuccessStatusCode();
        }

        public async Task UploadAgreementAsync(int id, Stream fileStream, string fileName)
        {
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(fileStream);
            content.Add(streamContent, "file", fileName);

            var response = await _httpClient.PostAsync($"api/Contract/{id}/upload", content);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Upload failed: {error}");
            }
        }

        public async Task<(Stream Content, string FileName)?> DownloadAgreementAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/Contract/{id}/download");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            var fileName = response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ?? $"contract_{id}_agreement.pdf";
            return (stream, fileName);
        }
    }
}
