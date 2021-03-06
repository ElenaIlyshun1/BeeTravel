using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BeeTravel.Entities;
using BeeTravel.Helpers;
using BeeTravel.Models.ManageViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BeeTravel.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<DbUser> _userManager;
        private readonly SignInManager<DbUser> _signInManager;
        private readonly RoleManager<DbRole> _roleManager;
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _env;
        public ManageController(UserManager<DbUser> userManager,
            SignInManager<DbUser> signInManager,
            RoleManager<DbRole> roleManager,
            ILogger<ManageController> logger,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new IndexViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                FirstName = user.Firstname,
                LastName = user.Lastname,
                Image = user.Image

            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
                var user = await _userManager.GetUserAsync(User);
            if (model.PhotoBase64 != null)
            {
                var serverPath = _env.ContentRootPath;
                var folerName = "Uploads";
                var path = Path.Combine(serverPath, folerName);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string ext = ".jpg";
                string fileName = Path.GetRandomFileName() + ext;

                string filePathSave = Path.Combine(path, fileName);

                using (var stream = System.IO.File.Create(filePathSave))
                {
                    await model.PhotoBase64.CopyToAsync(stream);
                    user.Image = fileName;
                    await _userManager.UpdateAsync(user);
                }



            }

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var firstname = user.Firstname;
            if (model.FirstName != firstname)
            {
                user.Firstname = model.FirstName;
                await _userManager.UpdateAsync(user);
            }
            var lastname = user.Lastname;
            if (model.LastName != lastname)
            {
                user.Lastname = model.LastName;
                await _userManager.UpdateAsync(user);
            }
            var email = user.Email;
            if (model.Email != email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{user.Id}'.");
                }
            }

            var phoneNumber = user.PhoneNumber;
            if (model.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    throw new ApplicationException($"Unexpected error occurred setting phone number for user with ID '{user.Id}'.");
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
