using BeeTravel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Models.OrderViewModels
{
    public class OrderViewModel
    {
        public Tour Tour { get; set; }
        public DbUser User { get; set; }
    }
}
