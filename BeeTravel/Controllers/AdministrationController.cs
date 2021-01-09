using BeeTravel.Entities;
using BeeTravel.Models.AdministrationViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeeTravel.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly UserManager<DbUser> _userManager;
        private readonly RoleManager<DbRole> _roleManager;

        public AdministrationController(UserManager<DbUser> userManager,
            RoleManager<DbRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        //   public IActionResult Index() => View(_userManager.Users.ToList());
        public async Task<IActionResult> Index(string sortOrder,string currentFilter,string searchString,int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["EmailSortParm"] = sortOrder == "Email" ? "email_desc" : "Email";
            ViewData["CurrentFilter"] = searchString;
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            var users = from u in _userManager.Users
                           select u;
            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Lastname.Contains(searchString)
                                       || u.Firstname.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    users = users.OrderBy(u => u.Firstname);
                    break;
                case "Date":
                    users = users.OrderBy(u => u.CreateDate);
                    break;
                case "date_desc":
                    users = users.OrderByDescending(u => u.CreateDate);
                    break;
                case "Email":
                    users = users.OrderBy(u => u.Email);
                    break;
                case "email_desc":
                    users = users.OrderByDescending(u => u.Email);
                    break;
                default:
                    users = users.OrderBy(u => u.Firstname);
                    break;
            }
            int pageSize = 8;
            return View(await PaginatedList<DbUser>.CreateAsync(users.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        public IActionResult Create() => View();
        [HttpPost]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                DbUser user = new DbUser { Email = model.Email ,Firstname = model.Firstname, Lastname = model.Lastname, PhoneNumber = model.PhoneNumber, UserName = model.Email , CreateDate = DateTime.UtcNow};
                var result = await _userManager.CreateAsync(user, model.Password);
                result = _userManager.AddToRoleAsync(user, "User").Result;
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
        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var model = _userManager.Users
                .Select(x => new EditUserViewModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    UserName = x.UserName,
                    Firstname = x.Firstname,
                    Lastname = x.Lastname,
                    PhoneNumber = x.PhoneNumber
                })
                .SingleOrDefault(x => x.Id == id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                DbUser user = await _userManager.FindByIdAsync(model.Id.ToString());
                if (user != null)
                {
                    user.Email = model.Email;
                    user.UserName = model.Email;
                    user.Lastname = model.Lastname;
                    user.Firstname = model.Firstname;
                    user.PhoneNumber = model.PhoneNumber;


                    var result = await _userManager.UpdateAsync(user);
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
            }
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            DbUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
        // Admin Role
        public IActionResult RoleIndex() => View(_roleManager.Roles.ToList());
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // We just need to specify a unique role name to create a new role
                DbRole identityRole = new DbRole
                {
                    Name = model.RoleName,
                    RoleColor = model.RoleColor
                };

                // Saves the role in the underlying AspNetRoles table
                var result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("roleindex", "administration");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var model = new List<UserRolesViewModel>();

            foreach (var role in _roleManager.Roles.ToList())
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }

                model.Add(userRolesViewModel);
            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                return View(model);
            }

            return RedirectToAction("index", "administration");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }
            else
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("RoleIndex");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("RoleIndex");
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name,
                RoleColor = role.RoleColor
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model,string returnUrl)
        {
            var role = await _roleManager.FindByIdAsync(model.Id.ToString());

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;
                role.RoleColor = model.RoleColor;
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("RoleIndex");
                    }
                    
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        public async Task<IActionResult> ConfirmEmail(string id)
        {
            if (ModelState.IsValid)
            {
                DbUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    user.EmailConfirmed = true;


                    var result = await _userManager.UpdateAsync(user);
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
            }
            return View();
        }
    }

}
