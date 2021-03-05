using BeeTravel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Models.AdministrationViewModels
{
    public class DashboardViewModel
    {
        public List<DbUser> Users { get; set; }
        public List<Tour> Tours { get; set; }
    }
}
