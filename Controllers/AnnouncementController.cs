using Microsoft.AspNetCore.Mvc;
using SkylineHOA.Data;
using SkylineHOA.Models;
using System;
using System.Linq;

namespace SkylineHOA.Controllers
{
    public class AnnouncementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnnouncementController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Create(string title, string description, string audience)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(description))
            {
                TempData["Error"] = "Title and message are required.";
                return RedirectToAction("AdminDashboard", "Home");
            }

            if (string.IsNullOrEmpty(audience))
            {
                audience = "Both";
            }

            var newAnnouncement = new Announcement
            {
                Title = title,
                Message = description,
                Target = audience,
                CreatedAt = DateTime.Now
            };

            _context.Announcements.Add(newAnnouncement);
            _context.SaveChanges();

            TempData["Success"] = "Announcement posted successfully!";
            return RedirectToAction("AdminDashboard", "Home");
        }

        [HttpPost]
        public IActionResult Edit(int id, string title, string message, string audience)
        {
            var announcement = _context.Announcements.FirstOrDefault(a => a.Id == id);
            if (announcement == null)
            {
                return NotFound();
            }

            announcement.Title = title;
            announcement.Message = message;
            announcement.Target = audience;
            announcement.CreatedAt = DateTime.Now;

            _context.SaveChanges();

            TempData["Success"] = "Announcement updated successfully!";
            return RedirectToAction("AdminDashboard", "Home");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var announcement = _context.Announcements.FirstOrDefault(a => a.Id == id);
            if (announcement == null)
            {
                return NotFound();
            }

            _context.Announcements.Remove(announcement);
            _context.SaveChanges();

            TempData["Success"] = "Announcement deleted successfully!";
            return RedirectToAction("AdminDashboard", "Home");
        }
    }
}
