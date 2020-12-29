using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Models.AdministrationViewModels
{
    public class CreateRoleViewModel
    {
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}
