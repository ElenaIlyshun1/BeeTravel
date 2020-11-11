using BeeTravel.Interfaces;
using BeeTravel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Data.Repository
{
    public class TourRepository : ITourRepository
    {
        private readonly ApplicationDbContext context;
        public TourRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        public Tour CreateTour(Tour newTour)
        {
            context.Tours.Add(newTour);
            context.SaveChanges();
            return newTour;
        }

        public Tour DeleteTour(Tour tour)
        {
            context.Tours.Remove(tour);
            context.SaveChanges();
            return tour;
        }

        public Tour EditTour(Tour newTour)
        {
            var tour = context.Tours.Attach(newTour);
            tour.State = EntityState.Modified;
            context.SaveChanges();
            return newTour;
        }

        public IEnumerable<Tour> GetAllTours()
        {
            return context.Tours;
        }

        public Tour GetTourById(int id)
        {
            return context.Tours.Find(id);
        }
    }

}
