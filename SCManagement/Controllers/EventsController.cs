using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.EventService;
using SCManagement.Services.UserService;

namespace SCManagement.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IClubService _clubService;

        public EventsController(ApplicationDbContext context, IEventService eventService, IUserService userService, UserManager<User> userManager, IClubService clubService)
        {
            _context = context;
            _eventService = eventService;
            _userService = userService;
            _userManager = userManager;
            _clubService = clubService;
        }

        private string getUserIdFromAuthedUser()
        {
            //reminder: this does not query the db, it just gets the id from the token (claims)
            return _userManager.GetUserId(User);
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetPublicEvents();

            var userId = getUserIdFromAuthedUser();
            var role = await _userService.GetSelectedRole(userId);
            if (role != null)
            {
                var clubEvents = await _eventService.GetClubEvents(role.ClubId);
                events = events.Concat(clubEvents);
            }

            events.OrderBy(e => e.StartDate);

            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myEvent = await _eventService.GetEvent(id.Value);
            if (myEvent == null)
            {
                return NotFound();
            }

            //if event is not public need to check if user is in the club
            if (!myEvent.IsPublic)
            {
                var userRole = await _userService.GetUserWithRoles(getUserIdFromAuthedUser());
                if (userRole == null)
                {
                    return View("CustomError", "Error_Unauthorized");
                }
            }

            return View(myEvent);

        }


        // GET: Events/Create
        public async Task<IActionResult> Create()
        {

            var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());
            if (role == null)
            {
                return View("CustomError", "Error_Unauthorized");

            }
            //Check if user is Staff of the club that owns the event
            if (!_clubService.IsClubStaff(role))
            {
                return View("CustomError", "Error_Unauthorized");
            }

            var EventResultTypes = from Event.ResultType e in Enum.GetValues(typeof(Event.ResultType))
                                   select new { Id = (int)e, Name = e.ToString() };

            ViewBag.EventResultType = new SelectList(EventResultTypes, "Id", "Name");
            return View();
        }


        //// POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,EndDate,Details,IsPublic,Fee,HaveRoute, EventResultType")] EventModel myEvent)
        {
            if (ModelState.IsValid)
            {
                var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());
                if (role == null)
                {
                    return View("CustomError", "Error_Unauthorized");

                }

                //Check if user is Staff of the club that owns the event
                if (!_clubService.IsClubStaff(role))
                {
                    return View("CustomError", "Error_Unauthorized");
                }

                await _eventService.CreateEvent(new Event
                {
                    Id = myEvent.Id,
                    Name = myEvent.Name,
                    StartDate = myEvent.StartDate,
                    EndDate = myEvent.EndDate,
                    Details = myEvent.Details,
                    IsPublic = myEvent.IsPublic,
                    Fee = myEvent.Fee,
                    HaveRoute = myEvent.HaveRoute,
                    EventResultType = myEvent.EventResultType,
                    ClubId = role.ClubId
                });

                return RedirectToAction(nameof(Index));
            }

            var EventResultTypes = from Event.ResultType e in Enum.GetValues(typeof(Event.ResultType))
                                   select new { Id = (int)e, Name = e.ToString() };

            ViewBag.EventResultType = new SelectList(EventResultTypes, "Id", "Name");
            return View(myEvent);
        }


        public class EventModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Details { get; set; }
            public bool IsPublic { get; set; }
            public double Fee { get; set; }
            public bool HaveRoute { get; set; }
            public Event.ResultType EventResultType { get; set; }

        }


        //// GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myEvent = await _eventService.GetEvent((int)id);
            if (myEvent == null)
            {
                return NotFound();
            }
            var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());
            if (!_clubService.IsClubStaff(role))
            {
                return View("CustomError", "Error_Unauthorized");
            }

            
            var EventResultTypes = from Event.ResultType e in Enum.GetValues(typeof(Event.ResultType))
                                   select new { Id = (int)e, Name = e.ToString() };

            ViewBag.EventResultType = new SelectList(EventResultTypes, "Id", "Name");
            return View(myEvent);
            
        }

        //// POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate,Details,IsPublic,Fee,HaveRoute,EventResultType")] EventModel myEvent)
        {
            if (id != myEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                //check if user is staff of the club that owns the event
                var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());
                if (!_clubService.IsClubStaff(role))
                {
                    return View("CustomError", "Error_Unauthorized");
                }

                var eventToUpdate = await _eventService.GetEvent(id);
                eventToUpdate.Name = myEvent.Name;
                eventToUpdate.StartDate = myEvent.StartDate;
                eventToUpdate.EndDate = myEvent.EndDate;
                eventToUpdate.Details = myEvent.Details;
                eventToUpdate.IsPublic = myEvent.IsPublic;
                eventToUpdate.Fee = myEvent.Fee;
                eventToUpdate.HaveRoute = myEvent.HaveRoute;
                eventToUpdate.EventResultType = myEvent.EventResultType;
                

                await _eventService.UpdateEvent(eventToUpdate);
                return RedirectToAction(nameof(Index));
            }

            
            var EventResultTypes = from Event.ResultType e in Enum.GetValues(typeof(Event.ResultType))
                                   select new { Id = (int)e, Name = e.ToString() };

            ViewBag.EventResultType = new SelectList(EventResultTypes, "Id", "Name");
            return View(myEvent);
        }

        //// GET: Events/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myEvent = await _eventService.GetEvent((int)id);
            if (myEvent == null)
            {
                return NotFound();
            }

            await _eventService.DeleteEvent(myEvent);

            return RedirectToAction(nameof(Index));
        }
    }
}
