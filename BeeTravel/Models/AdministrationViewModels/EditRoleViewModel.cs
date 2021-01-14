using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Models.AdministrationViewModels
{
    public class EditRoleViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Role Name is required")]
        public string RoleName { get; set; }
        public string RoleColor { get; set; }

    }
}
