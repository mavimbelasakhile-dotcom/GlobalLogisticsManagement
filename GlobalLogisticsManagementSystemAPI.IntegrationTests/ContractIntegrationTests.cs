using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace GlobalLogisticsManagementSystemAPI.IntegrationTests
{
    public class ContractIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ContractIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> GetAuthToken()
        {
            var username = "contractuser" + Guid.NewGuid().ToString("N")[..8];
            await _client.PostAsJsonAsync("/api/Auth/register", new
            {
                username,
                password = "Test@123",
                role = "Admin"
            });

            var response = await _client.PostAsJsonAsync("/api/Auth/login", new
            {
                username,
                password = "Test@123"
            });

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            return json.RootElement.GetProperty("token").GetString()!;
        }

        [Fact]
        public async Task GetContracts_WithAuth_Returns200OkAndJson()
        {
            var token = await GetAuthToken();
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/Contract");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);

            var json = JsonDocument.Parse(content);
            Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
        }

        [Fact]
        public async Task GetContracts_WithFiltering_Returns200Ok()
        {
            var token = await GetAuthToken();
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/Contract?status=Active");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostContract_WithAuth_ReturnsCreated()
        {
            var token = await GetAuthToken();

            // Create a client first
            using var clientRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Client");
            clientRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            clientRequest.Content = JsonContent.Create(new
            {
                name = "Contract Test Client",
                contactDetails = "contract@test.com",
                region = "Europe"
            });
            var clientResponse = await _client.SendAsync(clientRequest);
            var clientJson = JsonDocument.Parse(await clientResponse.Content.ReadAsStringAsync());
            var clientId = clientJson.RootElement.GetProperty("id").GetInt32();

            // Create contract
            using var contractRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Contract");
            contractRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            contractRequest.Content = JsonContent.Create(new
            {
                clientId,
                startDate = "2025-01-01",
                endDate = "2025-12-31",
                status = 1,
                serviceLevel = "Premium"
            });
            var response = await _client.SendAsync(contractRequest);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task PatchContractStatus_WithValidStatus_ReturnsOk()
        {
            var token = await GetAuthToken();

            // Create a client
            using var clientRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Client");
            clientRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            clientRequest.Content = JsonContent.Create(new
            {
                name = "Patch Test Client",
                contactDetails = "patch@test.com",
                region = "Asia"
            });
            var clientResponse = await _client.SendAsync(clientRequest);
            var clientJson = JsonDocument.Parse(await clientResponse.Content.ReadAsStringAsync());
            var clientId = clientJson.RootElement.GetProperty("id").GetInt32();

            // Create a contract
            using var contractRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Contract");
            contractRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            contractRequest.Content = JsonContent.Create(new
            {
                clientId,
                startDate = "2025-01-01",
                endDate = "2025-12-31",
                status = 0,
                serviceLevel = "Standard"
            });
            var contractResponse = await _client.SendAsync(contractRequest);
            var contractJson = JsonDocument.Parse(await contractResponse.Content.ReadAsStringAsync());
            var contractId = contractJson.RootElement.GetProperty("id").GetInt32();

            // Patch the status
            using var patchRequest = new HttpRequestMessage(HttpMethod.Patch, $"/api/Contract/{contractId}/status");
            patchRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            patchRequest.Content = JsonContent.Create(new { status = "Active" });
            var patchResponse = await _client.SendAsync(patchRequest);

            Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);
        }
    }
}
