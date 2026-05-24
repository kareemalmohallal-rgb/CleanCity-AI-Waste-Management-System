using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.MVC.Models
{
    public class DeviceToken
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string Token { get; set; }
        public string DeviceType { get; set; }
        public bool IsActive { get; set; }
    }

}
