using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Models.AdminViewModels
{

    public class TourCreateVM
    {
        [Required]
        [MaxLength(300, ErrorMessage = "Name can't exceed 300 characters")]
        public string Name { get; set; }

        public IFormFile Img { get; set; }

        public IFormFile ImgLarge { get; set; }

        [Required]
        [MaxLength(1000, ErrorMessage = "Description can't exceed 1000 characters")]
        public string Description { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "DepartureTown can't exceed 30 characters")]
        public string DepartureTown { get; set; }

        [Required]
        public ushort Period { get; set; }

        public bool IsNightCrossing { get; set; }

        [Required]
        [MaxLength(300, ErrorMessage = "Countries can't exceed 300 characters")]
        public string Countries { get; set; }

        [Required]
        public ushort Price { get; set; }

        [Required]
        public string DepartureDate { get; set; }

        public bool isFavorite { get; set; }

        public virtual Category Category { get; set; }
    }
}
