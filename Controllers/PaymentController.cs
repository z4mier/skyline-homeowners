using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SkylineHOA.Models;
using SkylineHOA.Data;

namespace SkylineHOA.Controllers
{
    [Authorize]
    [ApiController] // ✅ Ensures proper model binding for [FromBody]
    [Route("[controller]/[action]")] // ✅ Matches /Payment/SubmitBill
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpPost]
        public IActionResult SubmitBill([FromBody] Bill model)
        {
            try
            {
                if (model == null)
                {
                    Console.WriteLine("❌ Received null model");
                    return Json(new { success = false, message = "No data received" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    Console.WriteLine("❌ ModelState Errors: " + string.Join(", ", errors));
                    return Json(new { success = false, message = "Validation error", errors });
                }

                model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.Status = "Pending";
                model.CreatedAt = DateTime.Now;

                _context.Bills.Add(model);
                _context.SaveChanges();

                Console.WriteLine("✅ Bill saved successfully.");
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 Exception occurred: " + ex.Message);
                return Json(new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult BillsAndPayments(int page = 1, int pageSize = 5)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var query = _context.Bills
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.CreatedAt);

            var paged = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(query.Count() / (double)pageSize);
            ViewBag.PageSize = pageSize;

            return View(paged);
        }
    }
}
