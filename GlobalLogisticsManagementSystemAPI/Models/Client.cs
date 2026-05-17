namespace GlobalLogisticsManagementSystemAPI.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactDetails { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
