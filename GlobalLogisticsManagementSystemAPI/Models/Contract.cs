namespace GlobalLogisticsManagementSystemAPI.Models
{
    public enum ContractStatus
    {
        Draft,
        Active,
        Expired,
        OnHold
    }

    public class Contract
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ContractStatus Status { get; set; } = ContractStatus.Draft;
        public string ServiceLevel { get; set; } = string.Empty;
        public string? SignedAgreementPath { get; set; }

        // Navigation properties
        public Client? Client { get; set; }
        public ICollection<ServiceRequest>? ServiceRequests { get; set; }
    }
}

