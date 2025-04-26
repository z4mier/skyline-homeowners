using Microsoft.AspNetCore.Mvc;
using SkylineHOA.Data;
using SkylineHOA.Models;
using System.Linq;

namespace SkylineHOA.Controllers
{
    [Route("Event")]
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Event newEvent)
        {
            if (ModelState.IsValid)
            {
                _context.Events.Add(newEvent);
                _context.SaveChanges();

                return RedirectToAction("EventCalendar", "Admin", new
                {
                    month = newEvent.Date.Month,
                    year = newEvent.Date.Year
                });
            }

            return RedirectToAction("EventCalendar", "Admin");
        }

        [HttpPost("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Event updatedEvent)
        {
            if (ModelState.IsValid)
            {
                var eventInDb = _context.Events.FirstOrDefault(e => e.Id == updatedEvent.Id);
                if (eventInDb != null)
                {
                    eventInDb.Title = updatedEvent.Title;
                    eventInDb.Date = updatedEvent.Date;
                    eventInDb.StartTime = updatedEvent.StartTime;
                    eventInDb.EndTime = updatedEvent.EndTime;
                    eventInDb.Venue = updatedEvent.Venue;
                    eventInDb.Description = updatedEvent.Description;
                    _context.SaveChanges();

                    return RedirectToAction("EventCalendar", "Admin", new
                    {
                        month = updatedEvent.Date.Month,
                        year = updatedEvent.Date.Year
                    });
                }
            }

            return RedirectToAction("EventCalendar", "Admin");
        }

        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var eventToDelete = _context.Events.FirstOrDefault(e => e.Id == id);
            if (eventToDelete != null)
            {
                _context.Events.Remove(eventToDelete);
                _context.SaveChanges();

                return RedirectToAction("EventCalendar", "Admin", new
                {
                    month = eventToDelete.Date.Month,
                    year = eventToDelete.Date.Year
                });
            }

            return RedirectToAction("EventCalendar", "Admin");
        }
    }
}
