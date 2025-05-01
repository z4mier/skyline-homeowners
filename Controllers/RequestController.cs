using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SkylineHOA.Data;
using SkylineHOA.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace SkylineHOA.Controllers
{
    [Authorize]
    [Route("Request")]
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: /Request/Create
        [HttpPost("Create")]
        public IActionResult Create([FromBody] Request model)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized("User not logged in.");
            }

            model.UserId = userId;
            model.DateSubmitted = DateTime.Now;
            model.Status = "Pending";

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new
                    {
                        Field = x.Key,
                        Error = x.Value.Errors.First().ErrorMessage
                    });

                return BadRequest(new { message = "Validation failed", errors });
            }

            _context.Requests.Add(model);
            _context.SaveChanges();

            return Ok(new { message = "Request submitted successfully!" });
        }

        // GET: /Request/UserRequests
        [HttpGet("UserRequests")]
        public IActionResult UserRequests(int page = 1, int pageSize = 5)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var allRequests = _context.Requests
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.DateSubmitted)
                .ToList();

            int totalRequests = allRequests.Count;
            int totalPages = (int)Math.Ceiling((double)totalRequests / pageSize);

            var pagedRequests = allRequests
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            return View("UserRequests", pagedRequests);
        }
    }
}
