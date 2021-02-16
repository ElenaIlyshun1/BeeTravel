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
using MimeKit;

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
                    Image = "default_user.png"
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    // генерация токена для пользователя
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var code = await _userManager.GeneratePasswordResetTokenAsync(users.FirstOrDefault());
                    var callbackUrl = Url.Action(
                            "ConfirmEmail",
                            "Account",
                            new { userId = user.Id, code = code },
                            protocol: HttpContext.Request.Scheme);

                    var webRoot = _env.WebRootPath; //get wwwroot Folder  

                    //Get TemplateFile located at wwwroot/Templates/EmailTemplate/Register_EmailTemplate.html  
                    var pathToFile = _env.WebRootPath
                            + Path.DirectorySeparatorChar.ToString()
                            + "Templates"
                            + Path.DirectorySeparatorChar.ToString()
                            + "EmailTemplate"
                            + Path.DirectorySeparatorChar.ToString()
                            + "Confirm_Account_Registration.html";

                    var subject = "Confirm Account Registration";

                    var builder = new BodyBuilder();
                    using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                    {
                        builder.HtmlBody = SourceReader.ReadToEnd();
                    }
                    //{0} : Subject  
                    //{1} : DateTime  
                    //{2} : Email  
                    //{3} : Username  
                    //{4} : Password  
                    //{5} : Message  
                    //{6} : callbackURL  

                    string messageBody = string.Format(builder.HtmlBody,
                           subject,
                           String.Format("{0:dddd, d MMMM yyyy}", DateTime.Now),
                           model.Email,
                           model.Firstname,
                           model.Lastname,
                           model.Password,
                           callbackUrl
                           );

                    //ConfirmEmailCallbackLink(user.Id.ToString(), code, Request.Scheme);

                    // callbackUrl += $"&email={WebUtility.UrlEncode(user.Email)}";

                    await _emailSender.SendEmailAsync(model.Email, subject, messageBody);
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Щось пішло не так.");
                }
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");
            else
                return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return View("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                var webRoot = _env.WebRootPath; 

                var pathToFile = _env.WebRootPath
                        + Path.DirectorySeparatorChar.ToString()
                        + "Templates"
                        + Path.DirectorySeparatorChar.ToString()
                        + "EmailTemplate"
                        + Path.DirectorySeparatorChar.ToString()
                        + "Forgot_Password_Confirmation.html";

                var subject = "Confirm Reset Password";

                var builder = new BodyBuilder();
                using (StreamReader SourceReader = System.IO.File.OpenText(pathToFile))
                {
                    builder.HtmlBody = SourceReader.ReadToEnd();
                }
                string messageBody = string.Format(builder.HtmlBody,
                       subject,
                       String.Format("{0:dddd, d MMMM yyyy}", DateTime.Now),
                       model.Email,
                       callbackUrl
                       );

                await _emailSender.SendEmailAsync(model.Email, subject, messageBody);
                return View("ForgotPasswordConfirmation");
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return View("ResetPasswordConfirmation");
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return View("ResetPasswordConfirmation");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }
}
