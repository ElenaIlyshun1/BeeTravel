using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BeeTravel.Entities;
using BeeTravel.Helpers;
using BeeTravel.Interfaces;
using BeeTravel.Models;
using BeeTravel.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BeeTravel.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<DbUser> _userManager;
        private readonly SignInManager<DbUser> _signInManager;
        private readonly RoleManager<DbRole> _roleManager;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailSender _emailSender;


        public AccountController(UserManager<DbUser> userManager,
            SignInManager<DbUser> signInManager,
            RoleManager<DbRole> roleManager,
            IEmailSender emailSender,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _env = env;
            _emailSender = emailSender;

        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    //================================
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        if (result.IsLockedOut)
                        {

                            ModelState.AddModelError("", "Дані вкажано не коректно");
                            return View();

                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return RedirectToAction("Index","Home");
                        }
                    }
                }
            }

            ModelState.AddModelError("", "Дані вкажано не коректно");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationViewModel model)
        {
            bool isEmailExist = await _userManager.FindByEmailAsync(model.Email) == null;
            if (!isEmailExist)
            {
                ModelState.AddModelError("Email", "Така пошта уже є. Думай ...");
            }

            if (!isEmailExist)
            {
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var user = new DbUser
                {
                    Firstname = model.Firstname,
                    Lastname = model.Lastname,
                    Email = model.Email,
                    UserName = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    CreateDate = DateTimeOffset.UtcNow,
                    Image = "default-user.png"
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    // генерация токена для пользователя
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var code = await _userManager.GeneratePasswordResetTokenAsync(users.FirstOrDefault());
                    var callbackUrl = Url.Action(
                                     action: "Login",//realize method ConfirmEmail instead Login
                                     controller: "Account",
                                     values: new { user.Id, code },
                                     protocol: Request.Scheme);
                    //ConfirmEmailCallbackLink(user.Id.ToString(), code, Request.Scheme);

                    callbackUrl += $"&email={WebUtility.UrlEncode(user.Email)}";

                    await _emailSender.SendEmailAsync(user.Email, "Підтверження пошти",
                        $"Ви можете підтвердити свій акаунт за посиланням нижче.<br />" +
                        $"<a href='{callbackUrl}'>Підтвердити акаунт</a>");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Щось пішло не так.");
                }
            }
            return View(model);
        }
    }
}
