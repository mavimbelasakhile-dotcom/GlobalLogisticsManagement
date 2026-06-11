using System.Net.Http.Json;
using GlobalLogisticsManagementUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace GlobalLogisticsManagementUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(config["ApiSettings:BaseUrl"] ?? "http://localhost:5110/");
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserEmail") != null)
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var payload = new { username = model.Email, password = model.Password };
                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", payload);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<TokenResponse>();
                    HttpContext.Session.SetString("UserEmail", model.Email);
                    HttpContext.Session.SetString("UserName", model.Email.Split('@')[0]);
                    HttpContext.Session.SetString("JwtToken", result?.Token ?? "");
                    return RedirectToAction("Index", "Home");
                }

                TempData["Error"] = "Invalid email or password.";
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the authentication service.";
            }

            return View(model);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        private class TokenResponse
        {
            public string Token { get; set; } = string.Empty;
        }
    }
}
