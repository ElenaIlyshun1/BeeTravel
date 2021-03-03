using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
using Microsoft.AspNetCore.Http; // Needed for the SetString and GetString extension methods
using MimeKit;
using Newtonsoft.Json;
using Rotativa;
using Rotativa.Options;
using System.Net.Mail;

namespace BeeTravel.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<DbUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ITourRepository _tourRepository;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWebHostEnvironment _env;
        public OrderController(IEmailSender emailSender,
               UserManager<DbUser> userManager,
            ITourRepository tourRepository,
            ApplicationDbContext applicationDbContext,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _tourRepository = tourRepository;
            _emailSender = emailSender;
            _applicationDbContext = applicationDbContext;
            _env = env;
        }

        [HttpPost]
        public IActionResult Confirm(OrderSubmitViewModel model)
        {
            var name = "Order";
            var str = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString(name, str);
            return View();
        }
        [HttpPost]
        public IActionResult SuccessOrder(OrderPaymentViewModel model)
        {
            var str = HttpContext.Session.GetString("Order");
            var data = JsonConvert.DeserializeObject<OrderSubmitViewModel>(str);
            var webRoot = _env.WebRootPath;

            var pathToFile = _env.WebRootPath
                    + Path.DirectorySeparatorChar.ToString()
                    + "Templates"
                    + Path.DirectorySeparatorChar.ToString()
                    + "EmailTemplate"
                    + Path.DirectorySeparatorChar.ToString()
                    + "SendOrder.html";

            var subject = "Thank for your order.";

            var builder = new BodyBuilder();
            using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
            }
            var tour = _tourRepository.GetTourById(data.IdTour);
            string messageBody = string.Format(builder.HtmlBody,
                   subject,
                   String.Format("{0:dddd, d MMMM yyyy}", DateTime.Now),
                   data.Firstname + " " + data.Lastname,
                   data.Email,
                   tour.Name,
                   tour.Countries,
                   tour.DepartureTown,
                   tour.Period,
                   model.CardHolder,
                   model.CardNumber
                   );
            _emailSender.SendEmailAsync(data.Email, subject, messageBody);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetOrder(int id)
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
