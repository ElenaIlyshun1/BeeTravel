using BeeTravel.Helpers;
using BeeTravel.Interfaces;
using BeeTravel.Models;
using BeeTravel.Models.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private ITourRepository _tourRepository;
        private IHostingEnvironment _hostingEnvironment;
        public AdminController(ITourRepository tourRepository, IHostingEnvironment hostingEnvironment)
        {
            _tourRepository = tourRepository;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ViewResult CreateTour()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTour(TourCreateVM model)
        {
            if (ModelState.IsValid)
            {
                string uniqImgName = ImageHelper.SaveImage(_hostingEnvironment, model.Img);
                string uniqImgLargeName = ImageHelper.SaveImage(_hostingEnvironment, model.ImgLarge);

                Tour newTour = new Tour
                {
                    Name = model.Name,
                    Description = model.Description,
                    DepartureTown = model.DepartureTown,
                    DepartureDate = model.DepartureDate,
                    Countries = model.Countries,
                    IsNightCrossing = model.IsNightCrossing,
                    isFavorite = model.isFavorite,
                    Period = model.Period,
                    Price = model.Price,
                    Img = uniqImgName,
                    ImgLarge = uniqImgLargeName,
                };

                _tourRepository.CreateTour(newTour);

                return Redirect("/Home/Index");
            }
            return View();
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteTour(int id)
        {
            Tour tour = _tourRepository.GetTourById(id);
            if (tour != null)
            {
                ImageHelper.DeleteImage(_hostingEnvironment, tour.Img);
                _tourRepository.DeleteTour(tour);
            }
            return Redirect("/Home/Index");
        }

        [HttpGet]
        public ViewResult EditTour(int id)
        {
            Tour tour = _tourRepository.GetTourById(id);
            if (tour != null)
            {
                TourEditVM tourEditVM = new TourEditVM
                {
                    Id = tour.Id,
                    Name = tour.Name,
                    Description = tour.Description,
                    DepartureTown = tour.DepartureTown,
                    DepartureDate = tour.DepartureDate,
                    Countries = tour.Countries,
                    IsNightCrossing = tour.IsNightCrossing,
                    isFavorite = tour.isFavorite,
                    Period = tour.Period,
                    Price = tour.Price,
                    ExistImgName = tour.Img,
                    ExistImgLargeName = tour.ImgLarge
                };
                return View(tourEditVM);
            }
            else
            {
                NotFound();
            }
            return View();
        }

        [HttpPost]
        public IActionResult EditTour(TourEditVM model)
        {
            if (ModelState.IsValid)
            {
                Tour newTour = new Tour
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    DepartureTown = model.DepartureTown,
                    DepartureDate = model.DepartureDate,
                    Countries = model.Countries,
                    IsNightCrossing = model.IsNightCrossing,
                    isFavorite = model.isFavorite,
                    Period = model.Period,
                    Price = model.Price,

                    Img = ChangePhoto(model.ExistImgName, model.Img),
                    ImgLarge = ChangePhoto(model.ExistImgLargeName, model.ImgLarge),
                };

                _tourRepository.EditTour(newTour);
                return Redirect("/Home/Index");
            }
            return View();
        }

        public string ChangePhoto(string old, IFormFile newPhoto)
        {
            //Якщо вибрали нове фото
            if (newPhoto != null)
            {
                if (old != null)
                {
                    ImageHelper.DeleteImage(_hostingEnvironment, old);
                }
                string imageName = ImageHelper.SaveImage(_hostingEnvironment, newPhoto);
                return imageName;
            }
            //Якщо не вибирали нове фото
            return old;
        }
    }
}
