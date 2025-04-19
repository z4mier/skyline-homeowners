using Microsoft.AspNetCore.Mvc;
using SkylineHOA.Data;
using SkylineHOA.Models;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System;

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
        public IActionResult Register(string Username, string Email, string Password, string ConfirmPassword)
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

            var user = new User
            {
                Username = Username,
                Email = Email,
                PasswordHash = HashPassword(Password),
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "Registered successfully!";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Login(string Username, string Password)
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

            TempData["Success"] = $"Welcome, {user.Username}!";
            return RedirectToAction("Dashboard", "Home"); 
        }

        [HttpPost]
        public IActionResult Logout()
        {
            TempData["Success"] = "You have logged out.";
            return RedirectToAction("Index", "Home");
        }



        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
