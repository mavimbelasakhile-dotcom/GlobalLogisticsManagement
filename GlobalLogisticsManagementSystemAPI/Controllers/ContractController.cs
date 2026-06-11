using GlobalLogisticsManagementSystemAPI.Data;
using GlobalLogisticsManagementSystemAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GlobalLogisticsManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContractController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ContractController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Contract (with filtering)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contract>>> GetContracts(
            [FromQuery] int? clientId,
            [FromQuery] ContractStatus? status,
            [FromQuery] string? serviceLevel)
        {
            var query = _context.Contracts.Include(c => c.Client).AsQueryable();

            if (clientId.HasValue)
                query = query.Where(c => c.ClientId == clientId.Value);

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            if (!string.IsNullOrEmpty(serviceLevel))
                query = query.Where(c => c.ServiceLevel.Contains(serviceLevel));

            return await query.ToListAsync();
        }

        // GET: api/Contract/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contract>> GetContract(int id)
        {
            var contract = await _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contract == null)
            {
                return NotFound();
            }

            return contract;
        }

        // PUT: api/Contract/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContract(int id, Contract contract)
        {
            if (id != contract.Id)
            {
                return BadRequest();
            }

            _context.Entry(contract).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContractExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Contract
        [HttpPost]
        public async Task<ActionResult<Contract>> PostContract(Contract contract)
        {
            _context.Contracts.Add(contract);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContract), new { id = contract.Id }, contract);
        }

        // DELETE: api/Contract/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
            {
                return NotFound();
            }

            _context.Contracts.Remove(contract);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContractExists(int id)
        {
            return _context.Contracts.Any(e => e.Id == id);
        }

        // PATCH: api/Contract/5/status (approve/decline)
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateContractStatus(int id, [FromBody] ContractStatusUpdateDto dto)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
                return NotFound();

            if (!Enum.TryParse<ContractStatus>(dto.Status, true, out var newStatus))
                return BadRequest($"Invalid status. Valid values: {string.Join(", ", Enum.GetNames<ContractStatus>())}");

            contract.Status = newStatus;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Contract status updated to '{newStatus}'.", contract });
        }

        // POST: api/Contract/5/upload
        [HttpPost("{id}/upload")]
        public async Task<IActionResult> UploadSignedAgreement(int id, IFormFile file)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
                return NotFound();

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Only PDF and Word (.doc, .docx) files are allowed.");

            var uploadsFolder = Path.Combine(_env.ContentRootPath, "Uploads");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"Contract_{id}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            contract.SignedAgreementPath = fileName;
            await _context.SaveChangesAsync();

            return Ok(new { message = "File uploaded successfully.", fileName });
        }

        // GET: api/Contract/5/download
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadSignedAgreement(int id)
        {
            var contract = await _context.Contracts.FindAsync(id);
            if (contract == null)
                return NotFound();

            if (string.IsNullOrEmpty(contract.SignedAgreementPath))
                return NotFound("No signed agreement uploaded for this contract.");

            var filePath = Path.Combine(_env.ContentRootPath, "Uploads", contract.SignedAgreementPath);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found on server.");

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
            var ext = Path.GetExtension(contract.SignedAgreementPath).ToLower();
            var contentType = ext switch
            {
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
            return File(bytes, contentType, contract.SignedAgreementPath);
        }
    }
}
