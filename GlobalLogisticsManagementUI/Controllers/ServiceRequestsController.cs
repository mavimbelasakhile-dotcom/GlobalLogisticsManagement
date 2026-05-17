using GlobalLogisticsManagementUI.Models;
using GlobalLogisticsManagementUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GlobalLogisticsManagementUI.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly IContractService _contractService;

        public ServiceRequestsController(IServiceRequestService serviceRequestService, IContractService contractService)
        {
            _serviceRequestService = serviceRequestService;
            _contractService = contractService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var requests = await _serviceRequestService.GetAllAsync();
                return View(requests);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the API. Please ensure the GLMS API is running.";
                return View(Enumerable.Empty<ServiceRequestViewModel>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var request = await _serviceRequestService.GetByIdAsync(id);
                if (request == null) return NotFound();
                return View(request);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the API.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                var contracts = await _contractService.GetAllAsync();
                ViewBag.Contracts = contracts.Where(c => c.Status == 1);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the API. Cannot load contracts.";
                ViewBag.Contracts = Enumerable.Empty<ContractViewModel>();
            }
            return View(new ServiceRequestCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ServiceRequestCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                try
                {
                    var contracts = await _contractService.GetAllAsync();
                    ViewBag.Contracts = contracts.Where(c => c.Status == 1);
                }
                catch { ViewBag.Contracts = Enumerable.Empty<ContractViewModel>(); }
                return View(model);
            }

            try
            {
                await _serviceRequestService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message.Contains("Error") ? ex.Message : "Unable to connect to the API.";
                try
                {
                    var contracts = await _contractService.GetAllAsync();
                    ViewBag.Contracts = contracts.Where(c => c.Status == 1);
                }
                catch { ViewBag.Contracts = Enumerable.Empty<ContractViewModel>(); }
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _serviceRequestService.DeleteAsync(id);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the API.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
