using BeeTravel.Interfaces;
using BeeTravel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Controllers
{
    public class TourController : Controller
    {
        private readonly ITourRepository _tourRepository;
        private readonly ILogger logger;
        public TourController(ITourRepository tourRepository, ILogger<TourController> logger)
        {
            _tourRepository = tourRepository;
            this.logger = logger;
        }
        [Route("Tour/Index/{id}")]
        public IActionResult Tour(int id)
        {
            var tour = _tourRepository.GetTourById(id);

            return View(tour);
        }
        public IActionResult Index()
        {
            List<Tour> Tours = _tourRepository.GetAllTours().ToList();
            return View(Tours);
        }
    }
}
