using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Packaging;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.EventService;
using SCManagement.Services.PaymentService;
using SCManagement.Services.TranslationService;
using SCManagement.Services.UserService;

namespace SCManagement.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;
        private readonly IClubService _clubService;
        private readonly IPaymentService _paymentService;
        private readonly ITranslationService _translationService;

        public EventsController(
            IEventService eventService,
            IUserService userService,
            UserManager<User> userManager,
            IClubService clubService,
            IPaymentService paymentService,
            ITranslationService translationService
            )
        {
            _eventService = eventService;
            _userService = userService;
            _userManager = userManager;
            _clubService = clubService;
            _paymentService = paymentService;
            _translationService = translationService;
        }

        private string getUserIdFromAuthedUser()
        {
            //reminder: this does not query the db, it just gets the id from the token (claims)
            return _userManager.GetUserId(User);
        }


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


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("CustomError", "Error_NotFound");
            }

            var myEvent = await _eventService.GetEvent(id.Value);
            if (myEvent == null)
            {
                return View("CustomError", "Error_NotFound");
            }

            var userId = getUserIdFromAuthedUser();
            var userRole = await _userService.GetSelectedRole(userId);
            //if event is not public need to check if user is in the club
            if (!myEvent.IsPublic)
            {
                if (userRole == null)
                {
                    return View("CustomError", "Error_Unauthorized");
                }
            }

            //if users is staff can see users enrolled
            if (_clubService.IsClubStaff(userRole))
            {
                ViewBag.IsStaff = true;
            }

            //check if user is already enrolled
            var enroll = await _eventService.GetEnroll(myEvent.Id, userId);
            if (enroll != null)
            {
                ViewBag.IsEnrolled = true;
                ViewBag.IsPaid = enroll.EnrollStatus == EnrollPaymentStatus.Valid;
            }
            else
            {
                ViewBag.IsEnrolled = false;
            }

            string cultureInfo = Thread.CurrentThread.CurrentCulture.Name;

            myEvent.EventTranslations = myEvent.EventTranslations!.Where(cc => cc.Language == cultureInfo).ToList();

            return PartialView("_PartialEventDetails", myEvent);

        }


        [Authorize]
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

            var EventResultTypes = from ResultType e in Enum.GetValues(typeof(ResultType))
                                   select new { Id = (int)e, Name = e.ToString() };

            ViewBag.EventResultType = new SelectList(EventResultTypes, "Id", "Name");

            ViewBag.CultureInfo = Thread.CurrentThread.CurrentCulture.Name;

            var languages = new List<CultureInfo> { new("en-US"), new("pt-PT") };
            ViewBag.Languages = languages;

            List<EventTranslations> translations = new List<EventTranslations>();
            var eve = new Event();

            foreach (CultureInfo culture in languages)
            {
                translations.Add(new()
                {
                    EventId = eve.Id,
                    Value = "",
                    Language = culture.Name,
                    Atribute = "Details",
                });
            }

            eve.EventTranslations = new List<EventTranslations>(translations);

            return View(eve);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,EndDate,EventTranslations,IsPublic,Fee,HaveRoute,Route,EnrollLimitDate,EventResultType,MaxEventEnrolls,AddressByPath")] EventModel myEvent)
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

                var createdEvent = await _eventService.CreateEvent(new Event
                {
                    Id = myEvent.Id,
                    Name = myEvent.Name,
                    StartDate = myEvent.StartDate,
                    EndDate = myEvent.EndDate,
                    EventTranslations = myEvent.EventTranslations,
                    IsPublic = myEvent.IsPublic,
                    Fee = myEvent.Fee,
                    HaveRoute = myEvent.HaveRoute,
                    Route = myEvent.Route,
                    EventResultType = myEvent.EventResultType,
                    EnrollLimitDate = myEvent.EnrollLimitDate,
                    MaxEventEnrolls = myEvent.MaxEventEnrolls,
                    ClubId = role.ClubId,
                    AddressByPath = myEvent.AddressByPath
                });

                await UpdateTranslations(myEvent.EventTranslations, createdEvent);

                if (myEvent.Fee > 0)
                {
                    await _paymentService.CreateProductEvent(createdEvent);
                }

                return RedirectToAction(nameof(Index));
            }

            var EventResultTypes = from ResultType e in Enum.GetValues(typeof(ResultType))
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
            public DateTime EnrollLimitDate { get; set; }
            public ICollection<EventTranslations>? EventTranslations { get; set; }
            public bool IsPublic { get; set; }
            public float Fee { get; set; }
            public bool HaveRoute { get; set; }
            public string? Route { get; set; }
            public ResultType EventResultType { get; set; }
            public int MaxEventEnrolls { get; set; }
            public string? AddressByPath { get; set; }


        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("CustomError", "Error_NotFound");
            }

            var myEvent = await _eventService.GetEvent((int)id);
            if (myEvent == null)
            {
                return View("CustomError", "Error_NotFound");
            }
            var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());
            if (!_clubService.IsClubStaff(role))
            {
                return View("CustomError", "Error_Unauthorized");
            }


            var EventResultTypes = from ResultType e in Enum.GetValues(typeof(ResultType))
                                   select new { Id = (int)e, Name = e.ToString() };

            ViewBag.EventResultType = new SelectList(EventResultTypes, "Id", "Name");

            ViewBag.CultureInfo = Thread.CurrentThread.CurrentCulture.Name;
            ViewBag.Languages = new List<CultureInfo> { new("en-US"), new("pt-PT") };
            
            return View(myEvent);

        }

        private async Task UpdateTranslations(ICollection<EventTranslations> eventTranslations, Event actualEvent)
        {
            //update translations
            ICollection<EventTranslations> clubTranslationsFromFrontend = new List<EventTranslations>(eventTranslations);

            await _translationService.Translate(eventTranslations);

            foreach (var translations in clubTranslationsFromFrontend)
            {
                var f = actualEvent.EventTranslations.FirstOrDefault(c => c.Id == translations.Id);
                if (f != null)
                {
                    f.Value = translations.Value;
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate,EventTranslations,IsPublic,Fee,HaveRoute,Route,EnrollLimitDate,EventResultType,MaxEventEnrolls,AddressByPath")] EventModel myEvent)
        {
            if (id != myEvent.Id)
            {
                return View("CustomError", "Error_NotFound");
            }

            if (ModelState.IsValid)
            {
                //check if user is staff of the club that owns the event
                var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());
                if (!_clubService.IsClubStaff(role)) return View("CustomError", "Error_Unauthorized");

                var eventToUpdate = await _eventService.GetEvent(id);
                if (eventToUpdate == null) return View("CustomError", "Error_NotFound");

                eventToUpdate.Name = myEvent.Name;
                eventToUpdate.StartDate = myEvent.StartDate;
                eventToUpdate.EndDate = myEvent.EndDate;
                eventToUpdate.IsPublic = myEvent.IsPublic;
                eventToUpdate.Fee = myEvent.Fee;
                eventToUpdate.HaveRoute = myEvent.HaveRoute;
                eventToUpdate.Route = myEvent.Route;
                eventToUpdate.EventResultType = myEvent.EventResultType;
                eventToUpdate.EnrollLimitDate = myEvent.EnrollLimitDate;
                eventToUpdate.MaxEventEnrolls = myEvent.MaxEventEnrolls;
                eventToUpdate.AddressByPath = myEvent.AddressByPath;

                await UpdateTranslations(myEvent.EventTranslations, eventToUpdate);

                await _eventService.UpdateEvent(eventToUpdate);

                await _paymentService.UpdateProductEvent(eventToUpdate);

                return RedirectToAction(nameof(Index));
            }


            var EventResultTypes = from ResultType e in Enum.GetValues(typeof(ResultType))
                                   select new { Id = (int)e, Name = e.ToString() };

            ViewBag.EventResultType = new SelectList(EventResultTypes, "Id", "Name");
            return View(myEvent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return View("CustomError", "Error_NotFound");
            }

            var myEvent = await _eventService.GetEvent((int)id);
            if (myEvent == null)
            {
                return View("CustomError", "Error_NotFound");
            }

            await _eventService.DeleteEvent(myEvent);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EventEnrollment(int id)
        {
            var eventToEnroll = await _eventService.GetEvent(id);

            if (eventToEnroll == null) return View("CustomError", "Error_NotFound");

            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);

            //check if event is public
            if (!eventToEnroll.IsPublic && (role == null || role.ClubId != eventToEnroll.ClubId))
            {
                return View("CustomError", "Error_Unauthorized");
            }

            //check if user is already enrolled
            if (await _eventService.GetEnroll(id, userId) != null)
            {
                return View("CustomError", "Error_AlreadyEnrolled");
            }

            //create Enrollment
            var enroll = await _eventService.CreateEventEnroll(new EventEnroll
            {
                EventId = id,
                UserId = userId,
                EnrollDate = DateTime.Now,
                EnrollStatus = eventToEnroll.Fee != 0 ? EnrollPaymentStatus.Pending : EnrollPaymentStatus.Valid
            });

            //update event enroll users list
            eventToEnroll.UsersEnrolled.Add(enroll);

            await _eventService.UpdateEvent(eventToEnroll);
            var pay = await _paymentService.CreateEventPayment(enroll);

            if (pay != null) return RedirectToAction("Index", "Payment", new { payId = pay.Id });

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CancelEventEnroll(int? id)
        {
            if (id == null)
            {
                return View("CustomError", "Error_NotFound");
            }

            var myEvent = await _eventService.GetEvent((int)id);
            if (myEvent == null)
            {
                return View("CustomError", "Error_NotFound");
            }

            var userId = getUserIdFromAuthedUser();

            //Check if users is enrolled
            var enrollRoRemove = await _eventService.GetEnroll(myEvent.Id, userId);
            if (enrollRoRemove == null) return View("CustomError", "Error_NotFound");

            //Its only available to cancel enrollment if user not paid yet
            if (enrollRoRemove.EnrollStatus != EnrollPaymentStatus.Pending) return View("CustomError", "Error_Unauthorized");

            var removeCode = RemoveUserFromEnrollList(myEvent, userId);
            if (removeCode == -1) return View("CustomError", "Error_NotFound");

            await _eventService.CancelEventEnroll(enrollRoRemove);
            return RedirectToAction(nameof(Index));
        }

        private int RemoveUserFromEnrollList(Event myEvent, string userId)
        {
            var enrollToRemove = myEvent.UsersEnrolled.Where(u => u.UserId == userId).FirstOrDefault();

            if (enrollToRemove == null) return -1;

            myEvent.UsersEnrolled.Remove(enrollToRemove);
            return 0;
        }

        public async Task<IActionResult> PathInfoMapBox(int id)
        {
            var ev = await _eventService.GetEvent(id);

            if(ev == null) View("CustomError", "Error_NotFound");
            if (ev.Route == null) return View("CustomError", "Error_NotFound");
            return View(ev);
        }

    }
}
