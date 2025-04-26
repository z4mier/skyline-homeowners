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
    [Authorize]
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ViewRequests()
        {
            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            var userRequests = _context.Requests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.DateSubmitted)
                .ToList();

            return View(userRequests);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Request model)
        {
            var userIdClaim = User.FindFirst("UserID");
            if (userIdClaim == null)
                return Unauthorized();

            int userId = int.Parse(userIdClaim.Value);

            if (ModelState.IsValid)
            {
                model.UserId = userId;
                model.RequestId = Guid.NewGuid().ToString();
                model.DateSubmitted = DateTime.Now;
                model.Status = "Pending";
                model.DateUpdated = null;

                _context.Requests.Add(model);
                _context.SaveChanges();

                TempData["Success"] = "Request submitted successfully!";
                return RedirectToAction("ViewRequests");
            }

            return View(model);
        }
    }
}
