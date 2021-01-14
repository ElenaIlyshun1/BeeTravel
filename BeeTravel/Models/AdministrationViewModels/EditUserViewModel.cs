using BeeTravel.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Models.AdministrationViewModels
{
    public class EditUserViewModel
    {  
        public long Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string PhoneNumber { get; set; }

    }
}
