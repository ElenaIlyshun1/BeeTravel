using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
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
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                new { ReturnUrl = returnUrl });
            var properties = _signInManager
                .ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }
        [AllowAnonymous]
        public async Task<IActionResult>
            ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins =
                        (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState
                    .AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", loginViewModel);
            }

            // Get the login information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState
                    .AddModelError(string.Empty, "Error loading external login information.");

                return View("Login", loginViewModel);
            }

            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            // If there is no record in AspNetUserLogins table, the user may not have
            // a local account
            else
            {
                // Get the email claim value
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    var user = await _userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new DbUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Image = "default_user.png",
                            Firstname = info.Principal.FindFirstValue(ClaimTypes.GivenName),
                            Lastname = info.Principal.FindFirstValue(ClaimTypes.Surname),
                            PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone),
                            CreateDate = DateTimeOffset.UtcNow,
                            EmailConfirmed = true

                        };

                        await _userManager.CreateAsync(user);
                        await _userManager.AddToRoleAsync(user, "User");
                    }

                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // If we cannot find the user email we cannot continue
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

                return View("Error");
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins =
                (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
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
                callbackUrl += $"&email={WebUtility.UrlEncode(model.Email)}";

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
        public IActionResult ResetPassword(string code = null,string email = null)
        {
            ResetPasswordViewModel model = new ResetPasswordViewModel
            {
                Email = email,
                Code = code
            };


            //  return code == null ? View("Error") : View();
            return View(model);
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
