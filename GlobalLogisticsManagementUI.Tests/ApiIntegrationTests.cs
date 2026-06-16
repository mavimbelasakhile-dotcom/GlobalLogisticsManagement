using System.Net;
using System.Net.Http.Json;

namespace GlobalLogisticsManagementUI.Tests
{
    public class ApiIntegrationTests
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        public ApiIntegrationTests()
        {
            _baseUrl = "https://glmsapist10506152.azurewebsites.net/";
            _client = new HttpClient { BaseAddress = new Uri(_baseUrl) };
        }

        [Fact]
        public async Task GetContracts_ReturnsOk_AndJsonIsNotNull()
        {
            var response = await _client.GetAsync("api/Contract");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.NotEmpty(content);
        }

        [Fact]
        public async Task GetClients_ReturnsOk_AndJsonIsNotNull()
        {
            var response = await _client.GetAsync("api/Client");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.NotEmpty(content);
        }

        [Fact]
        public async Task GetServiceRequests_ReturnsOk_AndJsonIsNotNull()
        {
            var response = await _client.GetAsync("api/ServiceRequest");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.NotEmpty(content);
        }

        [Fact]
        public async Task GetContractById_WithValidId_ReturnsOk()
        {
            var response = await _client.GetAsync("api/Contract/3");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("clientId", content);
        }

        [Fact]
        public async Task GetContractById_WithInvalidId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("api/Contract/9999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PostServiceRequest_WithInvalidContract_ReturnsBadRequest()
        {
            var payload = new { contractId = 9999, description = "Test", costUsd = 100.00 };
            var response = await _client.PostAsJsonAsync("api/ServiceRequest", payload);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetSwagger_ReturnsOk()
        {
            var response = await _client.GetAsync("swagger/v1/swagger.json");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("openapi", content);
        }
    }
}
