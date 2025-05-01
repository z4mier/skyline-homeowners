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

        public IActionResult ServiceRequests(int page = 1, int pageSize = 5)
        {
            var query = from r in _context.Requests
                        join u in _context.Users on r.UserId equals u.UserID
                        orderby r.RequestId
                        select new RequestWithUser
                        {
                            RequestId = r.RequestId,
                            RequestType = r.RequestType,
                            Status = r.Status,
                            DateSubmitted = r.DateSubmitted,
                            UserName = (u.FirstName ?? "") + " " + (u.LastName ?? "")
                        };

            int totalItems = query.Count();
            var paginatedData = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.PageSize = pageSize;

            return View("~/Views/Admin/ServiceRequests.cshtml", paginatedData);
        }


        [HttpPost]
        public IActionResult UpdateRequestStatus(int id, string status)
        {
            var req = _context.Requests.FirstOrDefault(r => r.RequestId == id);
            if (req != null)
            {
                req.Status = status;
                req.ApprovedBy = User.Identity?.Name;
                _context.SaveChanges();
            }

            return RedirectToAction("ServiceRequests");
        }
    }
}
