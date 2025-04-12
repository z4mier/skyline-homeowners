using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SkylineHOA.Controllers
{
    public class AccountController : Controller
    {
        // This simulates a database. Replace with your actual DB logic
        private static List<User> users = new List<User>();

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password, string Role)
        {
            var user = users.FirstOrDefault(u => u.Username == Username && u.Password == Password && u.Role == Role);

            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, Username),
                    new Claim("FullName", user.FullName),
                    new Claim(ClaimTypes.Role, Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                TempData["LoginSuccess"] = $"{user.Role} logged in successfully!";
                return RedirectToAction("Dashboard", "Home");
            }

            TempData["LoginError"] = "Invalid credentials.";
            return Redirect("/");
        }

        [HttpPost]
        public IActionResult Register(string FullName, string Username, string Password, string ConfirmPassword, string Role)
        {
            if (Password != ConfirmPassword)
            {
                TempData["RegisterError"] = "Passwords do not match.";
                return Redirect("/");
            }

            if (users.Any(u => u.Username == Username))
            {
                TempData["RegisterError"] = "Username already exists.";
                return Redirect("/");
            }

            users.Add(new User { FullName = FullName, Username = Username, Password = Password, Role = Role });
            TempData["RegisterSuccess"] = $"{Role} registered successfully!";
            return Redirect("/");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }

    public class User
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}