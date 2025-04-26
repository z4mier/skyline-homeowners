using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkylineHOA.Models;
using SkylineHOA.Data;
using System.Security.Claims;
using System.Linq;

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

        [Authorize]
        public IActionResult Dashboard()
        {
            var role = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            if (role == "Admin")
                return RedirectToAction("AdminDashboard");

            if (role == "Staff")
                return RedirectToAction("StaffDashboard");

            return View();
        }

        [Authorize]
        public IActionResult AmenityBooking()
        {
            return View();
        }

        [Authorize]
        public IActionResult FormsAndContacts()
        {
            return View();
        }

        [Authorize]
        public IActionResult ViewContacts()
        {
            return View();
        }

        [Authorize]
        public IActionResult Announcements()
        {
            string userRole = User.IsInRole("Staff") ? "Staff" : "Residents";

            var announcements = _context.Announcements
                .Where(a => a.Target == userRole || a.Target == "Both")
                .OrderByDescending(a => a.CreatedAt)
                .ToList();

            ViewBag.Announcements = announcements;
            ViewBag.UserRole = userRole;

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            var announcements = _context.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToList();

            var totalResidents = _context.Users.Count(u => u.Role == "Resident");
            var totalStaffs = _context.Users.Count(u => u.Role == "Staff");

            ViewBag.Announcements = announcements;
            ViewBag.TotalResidents = totalResidents;
            ViewBag.TotalStaffs = totalStaffs;

            return View("~/Views/Admin/AdminDashboard.cshtml");
        }

        [Authorize(Roles = "Staff")]
        public IActionResult StaffDashboard()
        {
            return View("~/Views/Staff/StaffDashboard.cshtml");
        }

        [Authorize]
        public IActionResult UserEventCalendar(int? month, int? year)
        {
            int selectedMonth = month ?? DateTime.Now.Month;
            int selectedYear = year ?? DateTime.Now.Year;

            var allEvents = _context.Events.ToList();

            ViewBag.Events = allEvents;
            ViewBag.Month = selectedMonth;
            ViewBag.Year = selectedYear;

            return View("~/Views/Home/UserEventCalendar.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
