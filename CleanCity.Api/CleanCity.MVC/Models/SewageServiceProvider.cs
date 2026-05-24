using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.MVC.Models
{
    public class SewageServiceProvider
    {
        public int Id { get; set; }
        public string OwnerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }

        //public int AreaId { get; set; }
    }
}
