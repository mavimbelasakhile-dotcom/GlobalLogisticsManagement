namespace GlobalLogisticsManagementUI.Models
{
    public class ContractViewModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; } // 0=Draft, 1=Active, 2=Expired, 3=OnHold
        public string? ServiceLevel { get; set; }
        public string? SignedAgreementPath { get; set; }
        public ClientViewModel? Client { get; set; }

        public string StatusDisplay => Status switch
        {
            0 => "Draft",
            1 => "Active",
            2 => "Expired",
            3 => "On Hold",
            _ => "Unknown"
        };
    }

    public class ContractCreateViewModel
    {
        public int ClientId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Today;
        public DateTime EndDate { get; set; } = DateTime.Today.AddYears(1);
        public int Status { get; set; } = 0;
        public string? ServiceLevel { get; set; }
    }
}
