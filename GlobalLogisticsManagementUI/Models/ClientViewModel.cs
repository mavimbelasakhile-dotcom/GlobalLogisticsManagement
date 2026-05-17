namespace GlobalLogisticsManagementUI.Models
{
    public class ClientViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ContactDetails { get; set; }
        public string? Region { get; set; }
    }

    public class ClientCreateViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string? ContactDetails { get; set; }
        public string? Region { get; set; }
    }
}
