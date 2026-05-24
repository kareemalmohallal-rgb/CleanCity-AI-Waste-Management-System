using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Application.DTOs.SewageProviders
{
    public class CreateSewageServiceProviderDto
    {
        public string OwnerName { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Size { get; set; } = default!;
        public decimal Price { get; set; }
    }

}
