﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.EventService;
using SCManagement.Services.PaymentService;
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
        private readonly IPaymentService _paymentService;

        public EventsController(
            ApplicationDbContext context,
            IEventService eventService,
            IUserService userService,
            UserManager<User> userManager,
            IClubService clubService,
            IPaymentService paymentService
            )
        {
            _context = context;
            _eventService = eventService;
            _userService = userService;
            _userManager = userManager;
            _clubService = clubService;
            _paymentService = paymentService;
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
                return NotFound();
            }

            var myEvent = await _eventService.GetEvent(id.Value);
            if (myEvent == null)
            {
                return NotFound();
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

            return PartialView("_PartialEventDetails", myEvent); ;

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

            var EventResultTypes = from Event.ResultType e in Enum.GetValues(typeof(Event.ResultType))
                                   select new { Id = (int)e, Name = e.ToString() };

            ViewBag.EventResultType = new SelectList(EventResultTypes, "Id", "Name");
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,EndDate,Details,IsPublic,Fee,HaveRoute, EnrollLimitDate, EventResultType, MaxEventEnrolls")] EventModel myEvent)
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
                    Details = myEvent.Details,
                    IsPublic = myEvent.IsPublic,
                    Fee = myEvent.Fee,
                    HaveRoute = myEvent.HaveRoute,
                    EventResultType = myEvent.EventResultType,
                    EnrollLimitDate = myEvent.EnrollLimitDate,
                    MaxEventEnrolls = myEvent.MaxEventEnrolls,
                    ClubId = role.ClubId
                });

                if (myEvent.Fee > 0)
                {
                    await _paymentService.CreateProductEvent(createdEvent);
                }

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
            public DateTime EnrollLimitDate { get; set; }
            public string? Details { get; set; }
            public bool IsPublic { get; set; }
            public float Fee { get; set; }
            public bool HaveRoute { get; set; }
            public Event.ResultType EventResultType { get; set; }
            public int MaxEventEnrolls { get; set; }

        }

        [Authorize]
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


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate,Details,IsPublic,Fee,HaveRoute,EnrollLimitDate,EventResultType, MaxEventEnrolls")] EventModel myEvent)
        {
            if (id != myEvent.Id)
            {
                return NotFound();
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
                eventToUpdate.Details = myEvent.Details;
                eventToUpdate.IsPublic = myEvent.IsPublic;
                eventToUpdate.Fee = myEvent.Fee;
                eventToUpdate.HaveRoute = myEvent.HaveRoute;
                eventToUpdate.EventResultType = myEvent.EventResultType;
                eventToUpdate.EnrollLimitDate = myEvent.EnrollLimitDate;
                eventToUpdate.MaxEventEnrolls = myEvent.MaxEventEnrolls;

                await _eventService.UpdateEvent(eventToUpdate);

                await _paymentService.UpdateProductEvent(eventToUpdate);

                return RedirectToAction(nameof(Index));
            }


            var EventResultTypes = from Event.ResultType e in Enum.GetValues(typeof(Event.ResultType))
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
                return NotFound();
            }

            var myEvent = await _eventService.GetEvent((int)id);
            if (myEvent == null)
            {
                return NotFound();
            }

            var userId = getUserIdFromAuthedUser();

            //Check if users is enrolled
            var enrollRoRemove = await _eventService.GetEnroll(myEvent.Id, userId);
            if (enrollRoRemove == null) return NotFound();

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

    }
}