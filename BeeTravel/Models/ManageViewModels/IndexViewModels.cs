using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
        public string Image { get; set; }

        [Display(Name = "Оберіть фото")]
        [Required(ErrorMessage = "Оберіть фото профілю")]
        [HiddenInput]
        public IFormFile PhotoBase64 { get; set; }

        public string StatusMessage { get; set; }
    }
}