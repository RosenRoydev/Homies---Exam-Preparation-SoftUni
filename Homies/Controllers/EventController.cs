using Homies.Data;
using Homies.Models.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace Homies.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private readonly HomiesDbContext context;
        public EventController(HomiesDbContext _context)
        {
            context = _context;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var events = await context.Events
                .Select(e => new EventInfoViewModel(
                    e.Id,
                    e.Name,
                     e.CreatedOn.ToString(DataConstants.DateFormat),
                    e.Type.Name,
                    e.Organiser.UserName))
                .AsNoTracking()
                .ToListAsync();

            return View(events);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var eventForJoin = await context.Events
                .Where(e => e.Id == id)
                .Include(e => e.EventsParticipants)
                .FirstOrDefaultAsync();

            if (eventForJoin == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            if (!eventForJoin.EventsParticipants.Any(p => p.HelperId == userId))
            {
                eventForJoin.EventsParticipants.Add(new EventParticipant()
                {
                    EventId = eventForJoin.Id,
                    HelperId = userId
                });

                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            string userId = GetUserId();

            var model = await context.EventsParticipants
                .Where(ep => ep.HelperId == userId)
                .AsNoTracking()
                .Select(ep => new EventInfoViewModel(
                    ep.EventId,
                    ep.Event.Name,
                    ep.Event.Start.ToString(Data.DataConstants.DateFormat),
                    ep.Event.Type.Name,
                    ep.Event.Organiser.UserName
                    ))
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task <IActionResult> Add()
        {
            var model = new EventAddViewModel();
            model.Types = await GetTypes();

            return View(model);
        }

        private async Task <IEnumerable<TypeViewModel>> GetTypes()
        {
            return await context.Types
                .Select(t => new TypeViewModel()
                {
                    Id = t.Id,
                    Name = t.Name,
                })
                .AsNoTracking()
                .ToListAsync();



        }       

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
    }
}

