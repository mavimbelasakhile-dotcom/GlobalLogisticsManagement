namespace GlobalLogisticsManagementSystemAPI.Models
{
    public enum ServiceRequestStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }

    public class ServiceRequest
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal CostUsd { get; set; }
        public decimal CostZar { get; set; }
        public decimal ExchangeRate { get; set; }
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;

        // Navigation property
        public Contract? Contract { get; set; }
    }
}

