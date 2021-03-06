using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Models.OrderViewModels
{
    public class OrderSubmitViewModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int IdTour { get; set; }
        public DateTime DateDeparture { get; set; }

    }
}
