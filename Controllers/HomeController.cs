using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkylineHOA.Models;
using SkylineHOA.Data;
using System.Linq;
using System.Security.Claims;

namespace SkylineHOA.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize]
        public IActionResult AmenityBooking()
        {
            return View();
        }

        public IActionResult FormsAndContacts()
        {
            return View();
        }

        public IActionResult ViewContacts()
        {
            return View();
        }

        // ✅ Admin Dashboard
        [Authorize]
        public IActionResult AdminDashboard()
        {
            var role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            if (role != "Admin")
                return RedirectToAction("Dashboard");

            return View(); // Views/Home/AdminDashboard.cshtml
        }

        [Authorize]
        public IActionResult StaffDashboard()
        {
            var role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            // ⬇️ Temporary debug line
            TempData["DebugRole"] = $"Logged in as role: {role}";

            if (role != "Staff")
                return RedirectToAction("Dashboard");

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
