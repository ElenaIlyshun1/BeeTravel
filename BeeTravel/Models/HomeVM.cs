using BeeTravel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Models
{
    public class HomeVM
    {
        public List<DbUser> Users { get; set; }
        public SearchUser Search { get; set; }
    }
}
