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

        public IActionResult Contacts()
        {
            return View("~/Views/Admin/Contacts.cshtml"); 
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashboard()
        {
            ViewBag.TotalResidents = _context.Users.Count(u => u.Role == "Resident");
            ViewBag.TotalStaffs = _context.Users.Count(u => u.Role == "Staff");

            ViewBag.AmenityPending = _context.Bills
                .Where(b => !string.IsNullOrEmpty(b.AmenityName) && b.Status.Trim() == "Pending")
                .Count();

            ViewBag.AmenityApproved = _context.Bills
                .Where(b => !string.IsNullOrEmpty(b.AmenityName) && b.Status.Trim() == "Approved")
                .Count();

            ViewBag.AmenityDenied = _context.Bills
                .Where(b => !string.IsNullOrEmpty(b.AmenityName) && b.Status.Trim() == "Denied")
                .Count();

            ViewBag.PendingRequests = _context.Requests.Count(r => r.Status.Trim() == "Pending");
            ViewBag.ApprovedRequests = _context.Requests.Count(r => r.Status.Trim() == "Approved");
            ViewBag.DeniedRequests = _context.Requests.Count(r => r.Status.Trim() == "Denied");

            ViewBag.Announcements = _context.Announcements
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToList();

            return View("~/Views/Admin/AdminDashboard.cshtml");
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

            ViewBag.PendingRequests = _context.Requests.Count(r => r.Status == "Pending");
            ViewBag.ApprovedRequests = _context.Requests.Count(r => r.Status == "Approved");
            ViewBag.DeniedRequests = _context.Requests.Count(r => r.Status == "Denied");

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

        public IActionResult AmenityBookings(int page = 1, int pageSize = 5)
        {
            var query = from b in _context.Bills
                        where b.AmenityName != null
                        join u in _context.Users on b.UserId equals u.UserID.ToString()
                        orderby b.CreatedAt descending
                        select new BillWithUser
                        {
                            BillId = b.BillId,
                            AmenityName = b.AmenityName,
                            Status = b.Status,
                            BookingDate = b.BookingDate,
                            PaymentMethod = b.PaymentMethod,
                            UserName = u.FirstName + " " + u.LastName
                        };

            int totalItems = query.Count();
            var paginated = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.PageSize = pageSize;

            return View("~/Views/Admin/AmenityBookings.cshtml", paginated);
        }

        [HttpPost]
        public IActionResult UpdateBookingStatus(int id, string status)
        {
            var booking = _context.Bills.FirstOrDefault(b => b.BillId == id);
            if (booking != null)
            {
                booking.Status = status;
                _context.SaveChanges();
            }

            return RedirectToAction("AmenityBookings");
        }
        public IActionResult AdminAnnouncements(string sortOrder)
        {
            var announcements = _context.Announcements.ToList();
            ViewBag.Announcements = announcements;
            return View();
        }

    }
}
