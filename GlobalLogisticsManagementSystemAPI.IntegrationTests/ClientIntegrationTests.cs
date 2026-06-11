using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace GlobalLogisticsManagementSystemAPI.IntegrationTests
{
    public class ClientIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public ClientIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> GetAuthToken()
        {
            var username = "clientuser" + Guid.NewGuid().ToString("N")[..8];
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
        public async Task GetClients_WithoutAuth_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/Client");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetClients_WithAuth_ReturnsOkAndJsonArray()
        {
            var token = await GetAuthToken();
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/Client");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);

            var json = JsonDocument.Parse(content);
            Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
        }

        [Fact]
        public async Task PostClient_WithAuth_ReturnsCreated()
        {
            var token = await GetAuthToken();
            using var request = new HttpRequestMessage(HttpMethod.Post, "/api/Client");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(new
            {
                name = "Test Client",
                contactDetails = "test@test.com",
                region = "Africa"
            });

            var response = await _client.SendAsync(request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            Assert.True(json.RootElement.GetProperty("id").GetInt32() > 0);
        }
    }
}
