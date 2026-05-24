namespace CleanCity.MVC.Models
{
    public class DriverUserVm
    {
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public int? DriverId { get; set; }
        public string FullName { get; set; } = "—";
        public string PhoneNumber { get; set; } = "—";
        public string? LicenseNumber { get; set; }
        public string? AreaName { get; set; }
        public string? TruckName { get; set; }
        public bool IsLinked { get; set; }
    }
}