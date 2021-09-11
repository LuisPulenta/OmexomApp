using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GenericApp.Common.Enums;
using GenericApp.Common.Responses;
using GenericApp.Web.Data;
using GenericApp.Web.Data.Entities;
using GenericApp.Web.Helpers;
using GenericApp.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GenericApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IMailHelper _mailHelper;

        public AccountController(
            DataContext context,
            IUserHelper userHelper,
            ICombosHelper combosHelper,
            IImageHelper imageHelper,
            IMailHelper mailHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _imageHelper = imageHelper;
            _mailHelper = mailHelper;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users
                .Include(u => u.City)
                .Include(t => t.FavoriteTeam)
                .ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Create()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Countries = _combosHelper.GetComboCountries(),
                Departments = _combosHelper.GetComboDepartments(0),
                Cities = _combosHelper.GetComboCities(0),
                Teams = _combosHelper.GetComboTeams(0),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var imagePath = string.Empty;

                if (model.ImageFile != null)
                {
                    imagePath = await _imageHelper.UploadImageAsync(model.ImageFile, "users");
                }

                User user = await _userHelper.AddUserAsync(model, imagePath, UserType.Admin);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este mail ya está en uso.");
                    model.Countries = _combosHelper.GetComboCountries();
                    model.Departments = _combosHelper.GetComboDepartments(model.CountryId);
                    model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
                    return View(model);
                }

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                Response response = _mailHelper.SendMail(model.Username, "Confirmación de Email", $"<h1>Confirmación de Email</h1>" +
                    $"Para habilitar el usuario, " +
                    $"por favor haga clic en este link: </br></br><a href = \"{tokenLink}\">Confirmación de Email</a>");
                if (response.IsSuccess)
                {
                    ViewBag.Message = "Las instrucciones para habilitar su usuario han sido enviadas a su email.";
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, response.Message);
            }

            model.Countries = _combosHelper.GetComboCountries();
            model.Departments = _combosHelper.GetComboDepartments(model.CountryId);
            model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
            return View(model);
        }


        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model);
                if (result.Succeeded)
                {
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First());
                    }

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Email o password incorrecto.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult NotAuthorized()
        {
            return View();
        }

        public IActionResult Register()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Countries = _combosHelper.GetComboCountries(),
                Departments = _combosHelper.GetComboDepartments(0),
                Cities = _combosHelper.GetComboCities(0),
                Teams = _combosHelper.GetComboTeams(0),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var imagePath = string.Empty;
                if (model.ImageFile != null)
                {
                    imagePath = await _imageHelper.UploadImageAsync(model.ImageFile, "Users");
                }

                User user = await _userHelper.AddUserAsync(model, imagePath, UserType.User);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este mail ya está en uso.");
                    model.Countries = _combosHelper.GetComboCountries();
                    model.Departments = _combosHelper.GetComboDepartments(model.CountryId);
                    model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
                    return View(model);
                }

                string myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                string tokenLink = Url.Action("ConfirmEmail", "Account", new
                {
                    userid = user.Id,
                    token = myToken
                }, protocol: HttpContext.Request.Scheme);

                _mailHelper.SendMail(model.Username, "Confirmación de Email", $"<h1>Confirmación de Email</h1>" +
                    $"Para habilitar el usuario, " +
                    $"por favor haga clic en este link: </br></br><a href = \"{tokenLink}\">Confirmación de Email</a>");


                ViewBag.Message = "Las instrucciones para habilitar su usuario han sido enviadas a su email.";
                return View(model);

            }

            model.Countries = _combosHelper.GetComboCountries();
            model.Departments = _combosHelper.GetComboDepartments(model.CountryId);
            model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
            return View(model);
        }


        public JsonResult GetDepartments(int countryId)
        {
            CountryEntity country = _context.Countries
                .Include(c => c.Departments)
                .FirstOrDefault(c => c.Id == countryId);
            if (country == null)
            {
                return null;
            }

            return Json(country.Departments.OrderBy(d => d.Name));
        }

        public JsonResult GetTeams(int countryId)
        {
            CountryEntity country = _context.Countries
                .Include(c => c.Teams)
                .FirstOrDefault(c => c.Id == countryId);
            if (country == null)
            {
                return null;
            }

            return Json(country.Teams.OrderBy(d => d.Name));
        }

        public JsonResult GetCities(int departmentId)
        {
            DepartmentEntity department = _context.Departments
                .Include(d => d.Cities)
                .FirstOrDefault(d => d.Id == departmentId);
            if (department == null)
            {
                return null;
            }

            return Json(department.Cities.OrderBy(c => c.Name));
        }

        public async Task<IActionResult> ChangeUser()
        {
            User user = await _userHelper.GetUserAsync(User.Identity.Name);
            if (user == null)
            {
                return NotFound();
            }

            DepartmentEntity department = await _context.Departments.FirstOrDefaultAsync(d => d.Cities.FirstOrDefault(c => c.Id == user.City.Id) != null);
            if (department == null)
            {
                department = await _context.Departments.FirstOrDefaultAsync();
            }

            TeamEntity team = await _context.Teams.FirstOrDefaultAsync(c => c.Id == user.FavoriteTeam.Id);
            if (team == null)
            {
                team = await _context.Teams.FirstOrDefaultAsync();
            }

            CountryEntity country = await _context.Countries.FirstOrDefaultAsync(c => c.Departments.FirstOrDefault(d => d.Id == department.Id) != null);
            if (country == null)
            {
                country = await _context.Countries.FirstOrDefaultAsync();
            }

            CountryEntity country2 = await _context.Countries.FirstOrDefaultAsync(c => c.Teams.FirstOrDefault(d => d.Id == team.Id) != null);
            if (country2 == null)
            {
                country2 = await _context.Countries.FirstOrDefaultAsync();
            }

            EditUserViewModel model = new EditUserViewModel
            {
                Address = user.Address,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                PicturePath = user.PicturePath,
                Cities = _combosHelper.GetComboCities(department.Id),
                CityId = user.City.Id,
                Countries = _combosHelper.GetComboCountries(),
                CountryId = country.Id,
                CountryTeamId= country2.Id,
                DepartmentId = department.Id,
                Departments = _combosHelper.GetComboDepartments(country.Id),
                Teams = _combosHelper.GetComboTeams(country2.Id),
                TeamId = team.Id,
                Id = user.Id,
                Document = user.Document,
                UserTypes=_combosHelper.GetComboUserTypes(),
                UserType=user.UserType.ToString(),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var imagePath = model.PicturePath;
                if (model.ImageFile != null)
                {
                    imagePath = await _imageHelper.UploadImageAsync(model.ImageFile, "Users");
                }

                User user = await _userHelper.GetUserAsync(User.Identity.Name);

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.PicturePath = imagePath;
                user.City = await _context.Cities.FindAsync(model.CityId);
                user.FavoriteTeam = await _context.Teams.FindAsync(model.TeamId);
                user.Document = model.Document;
                await _userHelper.UpdateUserAsync(user);
                return RedirectToAction("Index", "Home");
            }

            model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
            model.Countries = _combosHelper.GetComboCountries();
            model.Departments = _combosHelper.GetComboDepartments(model.CityId);
            model.Teams = _combosHelper.GetComboTeams(model.CountryId);
            return View(model);
        }
        public IActionResult ChangePasswordMVC()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordMVC(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserAsync(User.Identity.Name);
                if (user != null)
                {
                    var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ChangeUser");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            User user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return NotFound();
            }

            return View();
        }
        public IActionResult RecoverPasswordMVC()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPasswordMVC(RecoverPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userHelper.GetUserAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Este mail no corresponde a un usuario registrado.");
                    return View(model);
                }

                string myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
                string link = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { token = myToken }, protocol: HttpContext.Request.Scheme);
                _mailHelper.SendMail(model.Email, "Reseteo de Password", $"<h1>Reseteo de Password</h1>" +
                    $"Para resetear el password por favor haga clic en este link: </br></br>" +
                    $"<a href = \"{link}\">Reset Password</a>");
                ViewBag.Message = "Las instrucciones para recuperar su password han sido enviadas a su email.";
                return View();

            }

            return View(model);
        }

        public IActionResult ResetPassword(string token)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            User user = await _userHelper.GetUserAsync(model.UserName);
            if (user != null)
            {
                IdentityResult result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    ViewBag.Message = "Password reseteado con éxito.";
                    return View();
                }

                ViewBag.Message = "Error mientras se reseteaba el password.";
                return View(model);
            }

            ViewBag.Message = "Usuario no encontrado.";
            return View(model);
        }
    }
}