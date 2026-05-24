namespace CleanCity.MVC.Models
{
    public class DriverIndexVm
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "—";
        public string? UserName { get; set; }
        public string? AreaName { get; set; }
        public string? TruckName { get; set; }
        public bool IsActive { get; set; }
    }
}