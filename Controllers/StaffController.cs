using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkylineHOA.Data;
using SkylineHOA.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace SkylineHOA.Controllers
{
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult DepartmentHeads()
        {

            return View();
        }

        public IActionResult Dashboard()
        {
            var announcements = _context.Announcements
                .Where(a => a.Target == "Staff" || a.Target == "Both")
                .OrderByDescending(a => a.CreatedAt)
                .AsNoTracking()
                .Take(5)
                .ToList();

            ViewBag.Announcements = announcements;
            return View("StaffDashboard");
        }

        public IActionResult AssignTask(int page = 1, int pageSize = 5)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the logged-in user's ID
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Login", "Account"); // If the user is not logged in, redirect them to login
            }

            // Fetch all requests for the logged-in staff member, including pending, approved, or denied, and unassigned requests
            var query = from r in _context.Requests
                        join u in _context.Users on r.UserId equals u.UserID
                        where string.IsNullOrEmpty(r.AssignedDepartment) // Requests that are not yet assigned
                        orderby r.DateSubmitted descending
                        select new RequestWithUser
                        {
                            RequestId = r.RequestId,
                            RequestType = r.RequestType,
                            Urgency = r.Urgency,
                            DateSubmitted = r.DateSubmitted,
                            UserName = u.FirstName + " " + u.LastName,
                            AssignedDepartment = r.AssignedDepartment,
                            Status = r.Status // Include status in the model
                        };

            // Total count of requests to calculate pagination
            int totalItems = query.Count();
            var paginated = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.PageSize = pageSize;

            return View(paginated);
        }

        [HttpPost]
        public IActionResult AssignRequest(int requestId, string assignedDepartment)
        {
            var request = _context.Requests.FirstOrDefault(r => r.RequestId == requestId);
            if (request != null && !string.IsNullOrEmpty(assignedDepartment))
            {
                request.AssignedDepartment = assignedDepartment; // Assign department
                _context.SaveChanges(); // Save changes to the database
            }

            return RedirectToAction("AssignTask"); // Redirect back to Assign Task page
        }

        public IActionResult ViewTasks()
        {
            var query = from r in _context.Requests
                        join u in _context.Users on r.UserId equals u.UserID
                        where r.AssignedDepartment != null // Filter by assigned department
                        orderby r.DateSubmitted descending
                        select new RequestWithUser
                        {
                            RequestId = r.RequestId,
                            RequestType = r.RequestType,
                            Status = r.Status,
                            Urgency = r.Urgency,
                            AssignedDepartment = r.AssignedDepartment,
                            DateSubmitted = r.DateSubmitted,
                            UserName = u.FirstName + " " + u.LastName
                        };

            var tasks = query.ToList();
            return View(tasks); // This now matches the View model
        }
    }
}
