using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace GlobalLogisticsManagementSystemAPI.IntegrationTests
{
    public class ServiceRequestIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ServiceRequestIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> GetAuthToken()
        {
            var username = "sruser" + Guid.NewGuid().ToString("N")[..8];
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
        public async Task GetServiceRequests_WithAuth_Returns200OkAndJson()
        {
            var token = await GetAuthToken();
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/ServiceRequest");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);

            var json = JsonDocument.Parse(content);
            Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
        }

        [Fact]
        public async Task PostServiceRequest_OnActiveContract_ReturnsCreated()
        {
            var token = await GetAuthToken();

            // Create client
            using var clientRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Client");
            clientRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            clientRequest.Content = JsonContent.Create(new
            {
                name = "SR Test Client",
                contactDetails = "sr@test.com",
                region = "Africa"
            });
            var clientResponse = await _client.SendAsync(clientRequest);
            var clientJson = JsonDocument.Parse(await clientResponse.Content.ReadAsStringAsync());
            var clientId = clientJson.RootElement.GetProperty("id").GetInt32();

            // Create active contract
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
            var contractResponse = await _client.SendAsync(contractRequest);
            var contractJson = JsonDocument.Parse(await contractResponse.Content.ReadAsStringAsync());
            var contractId = contractJson.RootElement.GetProperty("id").GetInt32();

            // Create service request
            using var srRequest = new HttpRequestMessage(HttpMethod.Post, "/api/ServiceRequest");
            srRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            srRequest.Content = JsonContent.Create(new
            {
                contractId,
                description = "Test delivery request",
                costUsd = 100.00,
                status = 0
            });
            var response = await _client.SendAsync(srRequest);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            Assert.True(json.RootElement.GetProperty("costZar").GetDecimal() > 0);
        }

        [Fact]
        public async Task PostServiceRequest_OnExpiredContract_ReturnsBadRequest()
        {
            var token = await GetAuthToken();

            // Create client
            using var clientRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Client");
            clientRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            clientRequest.Content = JsonContent.Create(new
            {
                name = "Expired Test Client",
                contactDetails = "expired@test.com",
                region = "Europe"
            });
            var clientResponse = await _client.SendAsync(clientRequest);
            var clientJson = JsonDocument.Parse(await clientResponse.Content.ReadAsStringAsync());
            var clientId = clientJson.RootElement.GetProperty("id").GetInt32();

            // Create expired contract
            using var contractRequest = new HttpRequestMessage(HttpMethod.Post, "/api/Contract");
            contractRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            contractRequest.Content = JsonContent.Create(new
            {
                clientId,
                startDate = "2024-01-01",
                endDate = "2024-12-31",
                status = 2,
                serviceLevel = "Basic"
            });
            var contractResponse = await _client.SendAsync(contractRequest);
            var contractJson = JsonDocument.Parse(await contractResponse.Content.ReadAsStringAsync());
            var contractId = contractJson.RootElement.GetProperty("id").GetInt32();

            // Try to create service request on expired contract
            using var srRequest = new HttpRequestMessage(HttpMethod.Post, "/api/ServiceRequest");
            srRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            srRequest.Content = JsonContent.Create(new
            {
                contractId,
                description = "Should fail",
                costUsd = 50.00,
                status = 0
            });
            var response = await _client.SendAsync(srRequest);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
