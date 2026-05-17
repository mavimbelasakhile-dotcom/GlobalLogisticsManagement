using GlobalLogisticsManagementUI.Models;
using GlobalLogisticsManagementUI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GlobalLogisticsManagementUI.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IContractService _contractService;
        private readonly IClientService _clientService;

        public ContractsController(IContractService contractService, IClientService clientService)
        {
            _contractService = contractService;
            _clientService = clientService;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? status)
        {
            try
            {
                var contracts = (await _contractService.GetAllAsync()).ToList();

                if (startDate.HasValue)
                    contracts = contracts.Where(c => c.StartDate >= startDate.Value).ToList();

                if (endDate.HasValue)
                    contracts = contracts.Where(c => c.EndDate <= endDate.Value).ToList();

                if (status.HasValue)
                    contracts = contracts.Where(c => c.Status == status.Value).ToList();

                ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
                ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
                ViewBag.Status = status;

                return View(contracts);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the API. Please ensure the GLMS API is running.";
                return View(Enumerable.Empty<ContractViewModel>());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var contract = await _contractService.GetByIdAsync(id);
                if (contract == null) return NotFound();
                return View(contract);
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
                var clients = await _clientService.GetAllAsync();
                ViewBag.Clients = clients;
            }
            catch (HttpRequestException)
            {
                ViewBag.Clients = Enumerable.Empty<ClientViewModel>();
                TempData["Error"] = "Unable to load clients from API.";
            }
            return View(new ContractCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                try { ViewBag.Clients = await _clientService.GetAllAsync(); }
                catch { ViewBag.Clients = Enumerable.Empty<ClientViewModel>(); }
                return View(model);
            }

            try
            {
                await _contractService.CreateAsync(model);
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message.Contains("Validation") ? ex.Message : "Unable to connect to the API.";
                try { ViewBag.Clients = await _clientService.GetAllAsync(); }
                catch { ViewBag.Clients = Enumerable.Empty<ClientViewModel>(); }
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var contract = await _contractService.GetByIdAsync(id);
                if (contract == null) return NotFound();
                var clients = await _clientService.GetAllAsync();
                ViewBag.Clients = clients;
                return View(contract);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the API.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContractViewModel model)
        {
            if (!ModelState.IsValid)
            {
                try { ViewBag.Clients = await _clientService.GetAllAsync(); }
                catch { ViewBag.Clients = Enumerable.Empty<ClientViewModel>(); }
                return View(model);
            }

            try
            {
                await _contractService.UpdateAsync(id, model);
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
                await _contractService.DeleteAsync(id);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to connect to the API.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a PDF file to upload.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase) &&
                !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Only PDF files are allowed.";
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                using var stream = file.OpenReadStream();
                await _contractService.UploadAgreementAsync(id, stream, file.FileName);
                TempData["Success"] = "Signed agreement uploaded successfully.";
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = ex.Message.Contains("Upload failed") ? ex.Message : "Unable to upload file.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Download(int id)
        {
            try
            {
                var result = await _contractService.DownloadAgreementAsync(id);
                if (result == null)
                {
                    TempData["Error"] = "No signed agreement found for this contract.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                return File(result.Value.Content, "application/pdf", result.Value.FileName);
            }
            catch (HttpRequestException)
            {
                TempData["Error"] = "Unable to download file.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}
