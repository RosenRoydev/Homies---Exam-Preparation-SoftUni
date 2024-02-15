using Homies.Data;
using Homies.Models.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Globalization;
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

        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            string userId = GetUserId();

            var eventForLeave = await context.Events
               .Where(e => e.Id == id)
               .Include(e => e.EventsParticipants)
               .FirstOrDefaultAsync();

            if (eventForLeave == null)
            {
                return BadRequest();
            }

            var participant = context.EventsParticipants
                .FirstOrDefault(p => p.HelperId == userId);

            if (participant == null)
            {
                return BadRequest();
            }

            eventForLeave.EventsParticipants.Remove(participant);

            await context.SaveChangesAsync();


            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new EventAddViewModel();
            model.Types = await GetTypes();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(EventAddViewModel model)
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;

            if (!DateTime.TryParseExact(model.Start,
                DataConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out start))
            {
                ModelState.AddModelError(nameof(model.Start),
                   $"The format for date must be {Data.DataConstants.DateFormat}");
            };

            if (!DateTime.TryParseExact(model.End,
                DataConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out start))
            {
                ModelState.AddModelError(nameof(model.End),
                   $"The format for date must be {Data.DataConstants.DateFormat}");
            };

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = new Data.Event()
            {
                Name = model.Name,
                CreatedOn = DateTime.Now,
                Description = model.Description,
                OrganiserId = GetUserId(),
                Start = start,
                End = end,
                TypeId = model.TypeId,
            };

            await context.Events.AddAsync(entity);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var eventForEdit = await context.Events
                .Where(e => e.Id == id)
                .Select(e => new EventAddViewModel
                {
                    Name = e.Name,
                    Description = e.Description,
                    Start = e.Start.ToString(Data.DataConstants.DateFormat),
                    End = e.End.ToString(Data.DataConstants.DateFormat),
                    TypeId = e.TypeId,

                })
                .AsNoTracking()
                .FirstOrDefaultAsync();



            if (eventForEdit == null)
            {
                return BadRequest();
            }
            eventForEdit.Types = await GetTypes();

            return View(eventForEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EventAddViewModel eventForEdit, int id)
        {

            var e = await context.Events
                .FindAsync(id);

            if (eventForEdit == null)
            {

                return BadRequest();
            }

            if (e.OrganiserId != GetUserId())
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                eventForEdit.Types = await GetTypes();
                return View(eventForEdit);
            }

            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;

            if (!DateTime.TryParseExact(eventForEdit.Start
                , Data.DataConstants.DateFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out start))
            {
                ModelState.AddModelError(eventForEdit.Start,
                    $"The date must be in format{Data.DataConstants.DateFormat}");
            }

            if (!DateTime.TryParseExact(eventForEdit.End
               , Data.DataConstants.DateFormat,
               CultureInfo.InvariantCulture,
               DateTimeStyles.None,
               out end))
            {
                ModelState.AddModelError(eventForEdit.End,
                    $"The date must be in format{Data.DataConstants.DateFormat}");
            }

            e.Name = eventForEdit.Name;
            e.Description = eventForEdit.Description;
            e.Start = start;
            e.End = end;
            e.TypeId = eventForEdit.TypeId;



            await context.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }

        public async Task <IActionResult> Details(int id)
        {
            var eventDetails = await context.Events
                .Where(e => e.Id == id)
                .Select(e => new EventDetailsViewModel
                {
                    Name = e.Name,
                    Description = e.Description,
                    Start = e.Start.ToString(Data.DataConstants.DateFormat),
                    End = e.End.ToString(Data.DataConstants.DateFormat),
                    Organiser = e.Organiser.UserName,
                    CreatedOn = e.CreatedOn,
                    TypeId = e.TypeId,
                    Type = e.Type.Name
      
                }).FirstOrDefaultAsync();
               
            if (eventDetails == null)
            {
                return RedirectToAction(nameof (All));
            }

            return View(eventDetails);
        }
        private async Task<IEnumerable<TypeViewModel>> GetTypes()
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

