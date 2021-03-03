using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BeeTravel.Data;
using BeeTravel.Entities;
using BeeTravel.Helpers;
using BeeTravel.Interfaces;
using BeeTravel.Models;
using BeeTravel.Models.AccountViewModels;
using BeeTravel.Models.OrderViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;

namespace BeeTravel.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<DbUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ITourRepository _tourRepository;
        private readonly ApplicationDbContext _applicationDbContext;
        public OrderController( IEmailSender emailSender,
               UserManager<DbUser> userManager,
            ITourRepository tourRepository,
            ApplicationDbContext applicationDbContext
            )
        {
            _userManager = userManager;
            _tourRepository = tourRepository;
            _emailSender = emailSender;
            _applicationDbContext = applicationDbContext;
        }

        [HttpPost]
        public IActionResult Confirm(OrderViewModel model)
        {
          
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>  GetOrder(int id)
        {
            var tour = _tourRepository.GetTourById(id);
            var user = await _userManager.GetUserAsync(User);
            var userapp = _applicationDbContext.Users.SingleOrDefault(x => x.Id == user.Id);
            var model = new OrderViewModel
            {
                Tour = tour,
                User = userapp
            };

            return View(model);
        }
        

    }
}
