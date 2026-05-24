using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanCity.Domain.Entities
{
    public class AnonymousDevice
    {
        public int Id { get; set; }

        // يولد أول مرة ويُخزن محليًا في التطبيق
        public string DeviceIdentifier { get; set; }

        // FCM Token
        public string FcmToken { get; set; }

        public DateTime CreatedAt { get; set; }
        public ICollection<Report>? Reports { get; set; } = new List<Report>();
    }
}
