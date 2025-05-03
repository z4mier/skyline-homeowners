using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SkylineHOA.Data;
using SkylineHOA.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace SkylineHOA.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Register(string FirstName, string LastName, string Username, string Email, string Password, string ConfirmPassword, string Role)
        {
            if (Password != ConfirmPassword)
            {
                TempData["Error"] = "Passwords do not match.";
                return RedirectToAction("Index", "Home");
            }

            if (_context.Users.Any(u => u.Username == Username || u.Email == Email))
            {
                TempData["Error"] = "Username or Email already exists.";
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(Role) || !(new[] { "Admin", "Staff", "User" }.Contains(Role)))
            {
                Role = "User";
            }

            var user = new User
            {
                FirstName = FirstName,
                LastName = LastName,
                Username = Username,
                Email = Email,
                PasswordHash = HashPassword(Password),
                Role = Role,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "Registered successfully!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Username, string Password)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                TempData["Error"] = "Please enter both username and password.";
                return RedirectToAction("Index", "Home");
            }

            var passwordHash = HashPassword(Password);
            var user = _context.Users.FirstOrDefault(u => u.Username == Username && u.PasswordHash == passwordHash);

            if (user == null)
            {
                TempData["Error"] = "Invalid username or password.";
                return RedirectToAction("Index", "Home");
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim("FullName", $"{user.FirstName} {user.LastName}"),
        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
        new Claim("UserID", user.UserID.ToString()),
        new Claim(ClaimTypes.Role, user.Role ?? "User")
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return user.Role switch
            {
                "Admin" => RedirectToAction("AdminDashboard", "Home"),
                "Staff" => RedirectToAction("StaffDashboard", "Home"),
                "User" => RedirectToAction("Dashboard", "Home"),
                _ => RedirectToAction("Dashboard", "Home")
            };
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "You have logged out.";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Profile()
        {
            string currentUsername = User.Identity?.Name;

            if (string.IsNullOrEmpty(currentUsername))
            {
                TempData["Error"] = "You are not logged in.";
                return RedirectToAction("Index", "Home");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == currentUsername);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult GenerateHash(string password = "admin123")
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return Content("Please provide a password to hash using ?password=yourpassword");
            }

            string hashed = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
            return Content($"Plain: {password}\nHashed: {hashed}");
        }

        [HttpPost]
        public IActionResult UpdateProfile(User updatedUser, string NewUsername, string NewPassword, string CurrentPassword)
        {
            string currentUsername = User.Identity?.Name;

            if (string.IsNullOrEmpty(currentUsername))
            {
                TempData["Error"] = "You are not logged in.";
                return RedirectToAction("Index", "Home");
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == currentUsername);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index", "Home");
            }

            if (!string.IsNullOrEmpty(CurrentPassword))
            {
                var hashed = HashPassword(CurrentPassword);
                if (user.PasswordHash != hashed)
                {
                    TempData["Error"] = "Incorrect current password.";
                    return RedirectToAction("Profile");
                }
            }

            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.ContactNumber = updatedUser.ContactNumber;
            user.Address = updatedUser.Address;

            if (!string.IsNullOrEmpty(NewUsername))
                user.Username = NewUsername;

            if (!string.IsNullOrEmpty(NewPassword))
                user.PasswordHash = HashPassword(NewPassword);

            _context.SaveChanges();

            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }

        private static string HashPassword(string password)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }
    }
}
