using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkylineHOA.Data;
using SkylineHOA.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkylineHOA.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult EventCalendar(int? month, int? year)
        {
            int currentMonth = month ?? DateTime.Now.Month;
            int currentYear = year ?? DateTime.Now.Year;

            var allEvents = _context.Events.ToList(); 

            ViewBag.Events = allEvents;
            ViewBag.Month = currentMonth;
            ViewBag.Year = currentYear;

            return View("~/Views/Admin/EventCalendar.cshtml");
        }
    }
}
