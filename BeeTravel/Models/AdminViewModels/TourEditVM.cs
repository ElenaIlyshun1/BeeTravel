using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Models.AdminViewModels
{
    public class TourEditVM : TourCreateVM
    {
        public int Id { get; set; }
        public string ExistImgName { get; set; }
        public string ExistImgLargeName { get; set; }
    }
}
