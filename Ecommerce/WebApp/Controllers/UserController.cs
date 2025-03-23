using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
//using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages; .NET 8.0
using WebApp.Models;
using WebApp.Security;
using WebApp.Viewmodels;
using System.Security.Claims;
using System.Text.Json;

namespace WebApp.Controllers
{
    public class UserController : Controller
    {
        private readonly EcommerceDwaContext _context;

        public UserController(EcommerceDwaContext context)
        {
            _context = context;
        }

        public IActionResult Login(string returnUrl)
        {
            //var claims = new List<Claim>();

            //var claimsIdentity = new ClaimsIdentity(
            //    claims,
            //    CookieAuthenticationDefaults.AuthenticationScheme);

            //var authProperties = new AuthenticationProperties();

            //Task.Run(async () =>
            //    await HttpContext.SignInAsync(
            //        CookieAuthenticationDefaults.AuthenticationScheme,
            //        new ClaimsPrincipal(claimsIdentity),
            //        authProperties)
            //).GetAwaiter().GetResult();

            //if (returnUrl != null)
            //{
            //    return LocalRedirect(returnUrl);
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home");
            //}

            var loginVm = new LoginVM { ReturnUrl = returnUrl};

            return View(loginVm);
        }

        [HttpPost]
        public IActionResult Login(LoginVM loginVm)
        {
            //if (login == null)
            //{
            //    throw new InvalidDataException();
            //}
            //if (login.Username == "test" && login.Password == "test")
            //{
            //    throw new Exception("test successful!");
            //}
            //return View();

            // Try to get a user from database
            var existingUser = _context.Users.Include(x => x.Role).FirstOrDefault(x => x.Username == loginVm.Username);
            if (existingUser == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            // Check is password hash matches
            var b64hash = PasswordHashProvider.GetHash(loginVm.Password, existingUser.PwdSalt);
            if (b64hash != existingUser.PwdHash)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            // Create proper cookie with claims
            var claims = new List<Claim>() {
            new Claim(ClaimTypes.Name, loginVm.Username),
            new Claim(ClaimTypes.Role, existingUser.Role.Name)
};

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();

            Task.Run(async () =>
              await HttpContext.SignInAsync(
                  CookieAuthenticationDefaults.AuthenticationScheme,
                  new ClaimsPrincipal(claimsIdentity),
                  authProperties)
            ).GetAwaiter().GetResult();


            if (existingUser.RoleId == 1)
            {
                return RedirectToAction("Search", "Product");
            }

            if (!string.IsNullOrEmpty(loginVm.ReturnUrl))
            {
                return LocalRedirect(loginVm.ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        public IActionResult Logout()
        {
            Task.Run(async () =>
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme)
            ).GetAwaiter().GetResult();

            return View();
        }

        public IActionResult AccessDenied(string returnUrl)
        {
            if (returnUrl == null)
            {
                return View();
            }
            else
            {
                return View();
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(UserVM userVm)
        {
            // Check if there is such a username in the database already
            var trimmedUsername = userVm.Username.Trim();
            if (_context.Users.Any(x => x.Username.Equals(trimmedUsername)))
            {
                ModelState.AddModelError("Username", $"Name {trimmedUsername} already exists");
                return View();
            }

            // Hash the password
            var b64salt = PasswordHashProvider.GetSalt();
            var b64hash = PasswordHashProvider.GetHash(userVm.Password, b64salt);

            // Create user from DTO and hashed password
            var user = new User
            {
                Id = userVm.Id,
                Username = userVm.Username,
                PwdHash = b64hash,
                PwdSalt = b64salt,
                FirstName = userVm.FirstName,
                LastName = userVm.LastName,
                Email = userVm.Email,
                Phone = userVm.Phone,
                RoleId = 2
            };

            // Add user and save changes to database
            _context.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login", "User");
        }

        [Authorize]
        public IActionResult ProfileDetails()
        {
            var username = HttpContext.User.Identity.Name;

            var userDb = _context.Users.First(x => x.Username == username);
            var userVm = new UserVM
            {
                Id = userDb.Id,
                Username = userDb.Username,
                FirstName = userDb.FirstName,
                LastName = userDb.LastName,
                Email = userDb.Email,
                Phone = userDb.Phone,
            };

            return View(userVm);
        }

        [Authorize]
        public IActionResult ProfileEdit(int id)
        {
            var userDb = _context.Users.First(x => x.Id == id);
            var userVm = new UserVM
            {
                Id = userDb.Id,
                Username = userDb.Username,
                FirstName = userDb.FirstName,
                LastName = userDb.LastName,
                Email = userDb.Email,
                Phone = userDb.Phone,
            };

            return View(userVm);
        }

        [Authorize]
        [HttpPost]
        public IActionResult ProfileEdit(int id, UserVM userVm)
        {
            var userDb = _context.Users.First(x => x.Id == id);
            userDb.FirstName = userVm.FirstName;
            userDb.LastName = userVm.LastName;
            userDb.Email = userVm.Email;
            userDb.Phone = userVm.Phone;

            _context.SaveChanges();

            return RedirectToAction("ProfileDetails");
        }

        public JsonResult GetProfileData(int id)
        {
            try
            {
                var userDb = _context.Users.First(x => x.Id == id);
                return Json(new
                {
                    userDb.FirstName,
                    userDb.LastName,
                    userDb.Email,
                    userDb.Phone,
                });
            }
            catch (Exception)
            {
                return Json(new { error = "Item not found" }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            }
        }

        [HttpPut]
        public ActionResult SetProfileData(int id, [FromBody] UserVM userVm)
        {
            var userDb = _context.Users.First(x => x.Id == id);
            userDb.FirstName = userVm.FirstName;
            userDb.LastName = userVm.LastName;
            userDb.Email = userVm.Email;
            userDb.Phone = userVm.Phone;

            _context.SaveChanges();

            return Ok();
        }
    }
}
