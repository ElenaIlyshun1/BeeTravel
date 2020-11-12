using BeeTravel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Interfaces
{
    interface ITourRepository
    {
        Tour GetTourById(int id);
        IEnumerable<Tour> GetAllTours();
        Tour CreateTour(Tour newTour);
        Tour DeleteTour(Tour tour);
        Tour EditTour(Tour newTour);
    }
}
