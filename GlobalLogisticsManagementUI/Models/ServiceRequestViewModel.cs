namespace GlobalLogisticsManagementUI.Models
{
    public class ServiceRequestViewModel
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string? Description { get; set; }
        public double CostUsd { get; set; }
        public double CostZar { get; set; }
        public double ExchangeRate { get; set; }
        public int Status { get; set; } // 0=Pending, 1=InProgress, 2=Completed, 3=Cancelled
        public ContractViewModel? Contract { get; set; }

        public string StatusDisplay => Status switch
        {
            0 => "Pending",
            1 => "In Progress",
            2 => "Completed",
            3 => "Cancelled",
            _ => "Unknown"
        };
    }

    public class ServiceRequestCreateViewModel
    {
        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public double CostUsd { get; set; }
        public int Status { get; set; } = 0;
    }
}
