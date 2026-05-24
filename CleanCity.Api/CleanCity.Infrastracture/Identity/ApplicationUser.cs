
using Microsoft.AspNetCore.Identity;
namespace CleanCity.Infrastracture.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public int? DriverId { get; set; }
    }
}
