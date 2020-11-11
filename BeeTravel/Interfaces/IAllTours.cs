using BeeTravel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Interfaces
{
    interface IAllTours
    {
        IEnumerable<Tour> Tours { get; set; }
        IEnumerable<Tour> getFavouriteTours { get; set; }
        Tour getObgectTour(int tourId);
    }
}
