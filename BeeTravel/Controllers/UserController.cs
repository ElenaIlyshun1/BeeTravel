using BeeTravel.Entities;
using BeeTravel.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Controllers
{
    public class UserController : Controller
    {
        UserManager<DbUser> _userManager;

        public UserController(UserManager<DbUser> userManager)
        {
            _userManager = userManager;
        }
        public IActionResult Index() => View(_userManager.Users.ToList());
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                DbUser user = new DbUser { Email = model.Email ,Firstname = model.Firstname, Lastname = model.Lastname, PhoneNumber = model.PhoneNumber};
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Edit(long id)
        {
            var model = _userManager.Users
                .Select(x=> new EditUserViewModel {
                    Id=x.Id,
                    Email=x.Email,
                    Firstname = x.Firstname,
                    Lastname = x.Lastname,
                    PhoneNumber = x.PhoneNumber
                })
                .SingleOrDefault(x=>x.Id==id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

    }
}
