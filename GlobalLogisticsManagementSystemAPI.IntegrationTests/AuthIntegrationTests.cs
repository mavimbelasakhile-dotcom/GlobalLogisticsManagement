using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace GlobalLogisticsManagementSystemAPI.IntegrationTests
{
    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsOk()
        {
            var response = await _client.PostAsJsonAsync("/api/Auth/register", new
            {
                username = "testuser",
                password = "Test@123",
                role = "User"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsToken()
        {
            // Register first
            await _client.PostAsJsonAsync("/api/Auth/register", new
            {
                username = "loginuser",
                password = "Test@123",
                role = "User"
            });

            // Login
            var response = await _client.PostAsJsonAsync("/api/Auth/login", new
            {
                username = "loginuser",
                password = "Test@123"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            Assert.True(json.RootElement.TryGetProperty("token", out _));
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var response = await _client.PostAsJsonAsync("/api/Auth/login", new
            {
                username = "nonexistent",
                password = "wrong"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
