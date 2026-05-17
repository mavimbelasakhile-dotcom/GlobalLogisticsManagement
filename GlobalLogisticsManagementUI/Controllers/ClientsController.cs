using GlobalLogisticsManagementUI.Models;
using GlobalLogisticsManagementUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GlobalLogisticsManagementUI.Controllers
{
    public class ClientsController : Controller
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var clients = await _clientService.GetAllAsync();
                return View(clients);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the API. Please ensure the GLMS API is running.";
                return View(Enumerable.Empty<ClientViewModel>());
            }
        }

        public IActionResult Create()
        {
            return View(new ClientCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _clientService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the API.";
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var client = await _clientService.GetByIdAsync(id);
                if (client == null) return NotFound();
                return View(client);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the API.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClientViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                await _clientService.UpdateAsync(id, model);
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the API.";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _clientService.DeleteAsync(id);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the API.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
