using GlobalLogisticsManagementUI.Models;
using GlobalLogisticsManagementUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GlobalLogisticsManagementUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContractService _contractService;
        private readonly IServiceRequestService _serviceRequestService;
        private readonly IClientService _clientService;

        public HomeController(IContractService contractService, IServiceRequestService serviceRequestService, IClientService clientService)
        {
            _contractService = contractService;
            _serviceRequestService = serviceRequestService;
            _clientService = clientService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var contracts = (await _contractService.GetAllAsync()).ToList();
                var requests = (await _serviceRequestService.GetAllAsync()).ToList();
                var clients = (await _clientService.GetAllAsync()).ToList();

                ViewBag.TotalClients = clients.Count;
                ViewBag.TotalContracts = contracts.Count;
                ViewBag.ActiveContracts = contracts.Count(c => c.Status == 1);
                ViewBag.TotalRequests = requests.Count;
                ViewBag.PendingRequests = requests.Count(r => r.Status == 0);
                ViewBag.RecentContracts = contracts.OrderByDescending(c => c.StartDate).Take(5);
                ViewBag.RecentRequests = requests.OrderByDescending(r => r.Id).Take(5);
            }
            catch (HttpRequestException)
            {
                ViewBag.TotalClients = 0;
                ViewBag.TotalContracts = 0;
                ViewBag.ActiveContracts = 0;
                ViewBag.TotalRequests = 0;
                ViewBag.PendingRequests = 0;
                ViewBag.RecentContracts = Enumerable.Empty<ContractViewModel>();
                ViewBag.RecentRequests = Enumerable.Empty<ServiceRequestViewModel>();
                TempData["Error"] = "Unable to connect to the API. Dashboard data unavailable.";
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
