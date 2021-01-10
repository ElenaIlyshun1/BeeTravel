using BeeTravel.Entities;
using BeeTravel.Helpers;
using BeeTravel.Interfaces;
using BeeTravel.Models;
using BeeTravel.Models.AdministrationViewModels;
using BeeTravel.Models.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly ITourRepository _tourRepository;
        private readonly UserManager<DbUser> _userManager;
        private readonly RoleManager<DbRole> _roleManager;
        private IHostingEnvironment _hostingEnvironment;

        public AdministrationController(UserManager<DbUser> userManager,
            RoleManager<DbRole> roleManager,ITourRepository tourRepository, IHostingEnvironment hostingEnvironment)
        {
            _tourRepository = tourRepository;
            _userManager = userManager;
            _roleManager = roleManager;
            _hostingEnvironment = hostingEnvironment;
        }
        #region User
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

        #endregion

        #region Role
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
        #endregion

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

        #region BanList
        public async Task<IActionResult> BanUser(string id)
        {
            if (ModelState.IsValid)
            {
                DbUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await _userManager.SetLockoutEnabledAsync(user, true);

                    var result = await _userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddMinutes(10));
                    
                    if(result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }

                }
            }
            return View();
        }
        public async Task<IActionResult> BanList(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["BlockSortParm"] = sortOrder == "Block" ? "block_desc" : "Block";
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
                users = users.Where(u => u.LockoutEnd >= DateTime.UtcNow);
                users = users.Where(u => u.Lastname.Contains(searchString)
                                       || u.Firstname.Contains(searchString));
            }
            else
            {
                users = users.Where(u => u.LockoutEnd >= DateTime.UtcNow);
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
                case "Block":
                    users = users.OrderBy(u => u.LockoutEnd >= DateTime.UtcNow);
                    break;
                case "block_desc":
                    users = users.OrderByDescending(u => u.LockoutEnd >= DateTime.UtcNow);
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
        public async Task<IActionResult> UnBanUser(string id, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                DbUser user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    if(await _userManager.IsLockedOutAsync(user))
                    {
                        var result =  await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                        if (result.Succeeded)
                        {
                            if (!string.IsNullOrEmpty(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return RedirectToAction("BanList");
                            } 
                        }
                    }
                  

                }
            }
            return View();
        }
        #endregion

        #region Tour
        public IActionResult TourList()
        {
            List<Tour> Tours = _tourRepository.GetAllTours().ToList();
            return View(Tours);
        }
        [Route("Administration/TourList/{id}")]
        public IActionResult OpenTour(int id)
        {
            var tour = _tourRepository.GetTourById(id);

            return View(tour);
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
        [HttpPost]
        public async Task<IActionResult> DeleteTour(int id,string returnUrl)
        {
            Tour tour = _tourRepository.GetTourById(id);
            if (tour != null)
            {
                ImageHelper.DeleteImage(_hostingEnvironment, tour.Img);
                _tourRepository.DeleteTour(tour);
            }
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("TourList");
            }
           
            
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
        #endregion
    }

}
