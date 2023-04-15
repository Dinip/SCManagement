using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SCManagement.Models;
using SCManagement.Services.ClubService;
using SCManagement.Services.EventService;
using SCManagement.Services.NotificationService;
using SCManagement.Services.PaymentService;
using SCManagement.Services.TranslationService;
using SCManagement.Services.UserService;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
        private readonly INotificationService _notificationService;

        public EventsController(
            IEventService eventService,
            IUserService userService,
            UserManager<User> userManager,
            IClubService clubService,
            IPaymentService paymentService,
            ITranslationService translationService,
            INotificationService notificationService
            )
        {
            _eventService = eventService;
            _userService = userService;
            _userManager = userManager;
            _clubService = clubService;
            _paymentService = paymentService;
            _translationService = translationService;
            _notificationService = notificationService;
        }

        private string getUserIdFromAuthedUser()
        {
            //reminder: this does not query the db, it just gets the id from the token (claims)
            return _userManager.GetUserId(User);
        }


        public async Task<IActionResult> Index(int? filterEvent)
        {
            var userId = getUserIdFromAuthedUser();

            var events = await _eventService.GetEvents(userId);

            //Check if iss Staff of the active club
            var role = await _userService.GetSelectedRole(userId);

            ViewBag.IsStaff = _clubService.IsClubStaff(role);

            switch (filterEvent)
            {
                case 1: // Em andamento
                    events = events.Where(e => e.StartDate <= DateTime.Now && e.EndDate >= DateTime.Now).ToList();
                    break;
                case 2: // Terminados
                    events = events.Where(e => e.EndDate < DateTime.Now).ToList();
                    break;
                case 3: // Futuros
                    events = events.Where(e => e.StartDate > DateTime.Now).ToList();
                    break;
                default: // Todos (0)
                    break;
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

            var userRole = await _clubService.GetUserRoleInClub(userId, myEvent.ClubId);
            //if event is not public need to check if user is in the club
            if (!myEvent.IsPublic && userRole.RoleId == 0)
            {
                return View("CustomError", "Error_Unauthorized");
            }

            //if users is staff can see users enrolled
            if (_clubService.IsClubStaff(userRole))
            {
                ViewBag.IsStaff = true;

            }
            ViewBag.Enrolls = await _eventService.GetEnrolls(myEvent.Id);
            //check if user is already enrolled
            var enroll = await _eventService.GetEnroll(myEvent.Id, userId);
            if (enroll != null)
            {
                ViewBag.IsEnrolled = true;
                ViewBag.IsPayed = (enroll.EnrollStatus == EnrollPaymentStatus.Valid || myEvent.Fee == 0);
                ViewBag.CanUnEnroll = (myEvent.Fee == 0 || enroll.EnrollStatus == EnrollPaymentStatus.Pending);
            }
            else
            {
                ViewBag.IsEnrolled = false;
            }

            string cultureInfo = Thread.CurrentThread.CurrentCulture.Name;
            myEvent.EventTranslations = myEvent.EventTranslations!.Where(cc => cc.Language == cultureInfo).ToList();

            return View("EventDetails", myEvent);
        }


        [Authorize]
        public async Task<IActionResult> Create()
        {
            var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());

            if (!_clubService.IsClubStaff(role))
            {
                return View("CustomError", "Error_Unauthorized");
            }

            ViewBag.ValidKey = await _paymentService.ClubHasValidKey(role.ClubId);

            var EventResultTypes = from ResultType e in Enum.GetValues(typeof(ResultType))
                                   select new { Id = (int)e, Name = e.ToString() };

            ViewBag.EventResultType = new SelectList(EventResultTypes, "Id", "Name");

            ViewBag.CultureInfo = Thread.CurrentThread.CurrentCulture.Name;

            var languages = new List<CultureInfo> { new("en-US"), new("pt-PT") };
            ViewBag.Languages = languages;

            var eve = new EventModel();
            eve.EventTranslationsName = new List<EventTranslation>();
            eve.EventTranslationsDetails = new List<EventTranslation>();

            foreach (CultureInfo culture in languages)
            {
                eve.EventTranslationsDetails.Add(new()
                {
                    EventId = eve.Id,
                    Value = "",
                    Language = culture.Name,
                    Atribute = "Details",
                });

                eve.EventTranslationsName.Add(new()
                {
                    EventId = eve.Id,
                    Value = "",
                    Language = culture.Name,
                    Atribute = "Name",
                });
            }

            return View(eve);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,StartDate,EndDate,EventTranslationsName,EventTranslationsDetails,IsPublic,Fee,HaveRoute,Route,EnrollLimitDate,EventResultType,MaxEventEnrolls,AddressByPath,LocationString")] EventModel myEvent)
        {
            var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());
            if (ModelState.IsValid)
            {
                //Check if user is Staff of the club that owns the event
                if (!_clubService.IsClubStaff(role))
                {
                    return View("CustomError", "Error_Unauthorized");
                }

                var validKey = await _paymentService.ClubHasValidKey(role.ClubId);

                Address newLocation = null;
                //Create Location Address
                if (myEvent.LocationString != null)
                {
                    Address location = JsonConvert.DeserializeObject<Address>(myEvent.LocationString);
                    if (location != null)
                    {
                        newLocation = await _eventService.CreateEventAddress(location);
                    }
                }

                if (myEvent.StartDate < DateTime.Now || myEvent.EndDate < myEvent.StartDate || myEvent.EnrollLimitDate > myEvent.StartDate || myEvent.EnrollLimitDate < DateTime.Now)
                {
                    return View("CustomError", "Error_InvalidInput");
                }

                var createdEvent = new Event
                {
                    StartDate = myEvent.StartDate,
                    EndDate = myEvent.EndDate,
                    IsPublic = myEvent.IsPublic,
                    Fee = validKey ? myEvent.Fee : 0,
                    HaveRoute = myEvent.HaveRoute,
                    Route = myEvent.Route,
                    LocationId = newLocation == null ? null : newLocation.Id,
                    EventResultType = myEvent.EventResultType,
                    EnrollLimitDate = myEvent.EnrollLimitDate,
                    MaxEventEnrolls = myEvent.MaxEventEnrolls == 0 ? int.MaxValue : myEvent.MaxEventEnrolls,
                    ClubId = role.ClubId,
                    AddressByPath = myEvent.AddressByPath,
                    EventTranslations = new List<EventTranslation>(),
                    CreationDate = DateTime.Now,
                };

                await UpdateTranslations(myEvent.EventTranslationsName, createdEvent);
                await UpdateTranslations(myEvent.EventTranslationsDetails, createdEvent);

                var translations = new List<EventTranslation>(myEvent.EventTranslationsName);
                translations.AddRange(myEvent.EventTranslationsDetails);
                createdEvent.EventTranslations = translations;

                await _eventService.CreateEvent(createdEvent);

                if (myEvent.Fee > 0)
                {
                    await _paymentService.CreateProductEvent(createdEvent);
                }

                _notificationService.NotifyEventCreate(createdEvent);

                return RedirectToAction(nameof(Index));
            }

            var EventResultTypes = from ResultType e in Enum.GetValues(typeof(ResultType))
                                   select new { Id = (int)e, Name = e.ToString() };

            ViewBag.ValidKey = await _paymentService.ClubHasValidKey(role.ClubId);

            ViewBag.EventResultType = new SelectList(EventResultTypes, "Id", "Name");
            return View(myEvent);
        }


        public class EventModel
        {
            public int Id { get; set; }
            [Display(Name = "Start Date")]
            public DateTime StartDate { get; set; }
            [Display(Name = "End Date")]
            public DateTime EndDate { get; set; }
            [Display(Name = "Enroll Limit Date")]
            public DateTime EnrollLimitDate { get; set; }
            public ICollection<EventTranslation>? EventTranslationsName { get; set; }
            public ICollection<EventTranslation>? EventTranslationsDetails { get; set; }
            [Display(Name = "Public Event")]
            public bool IsPublic { get; set; }
            [Display(Name = "Fee")]
            [Range(0, float.MaxValue, ErrorMessage = "Please enter a value between 0 and 2147483647")]
            public float Fee { get; set; }
            [Display(Name = "Event Have Route")]
            public bool HaveRoute { get; set; }
            public string? Route { get; set; }

            [Display(Name = "Event Result Type")]
            public ResultType EventResultType { get; set; }
            [Display(Name = "Max Enrolls")]
            [Range(0, int.MaxValue, ErrorMessage = "Please enter a value between 0 and 2147483647")]
            public int MaxEventEnrolls { get; set; }
            [Display(Name = "Event Location")]
            public string? AddressByPath { get; set; }
            public string? LocationString { get; set; }
            public int? LocationId { get; set; }
            public Address? Location { get; set; }

            public string? EventAux { get; set; }
            public DateTime? CreationDate { get; set; }
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

            if (myEvent.MaxEventEnrolls == int.MaxValue)
            {
                myEvent.MaxEventEnrolls = 0;
            }

            var userRole = await _clubService.GetUserRoleInClub(getUserIdFromAuthedUser(), myEvent.ClubId);
            if (!_clubService.IsClubStaff(userRole))
            {
                return View("CustomError", "Error_Unauthorized");
            }

            ViewBag.ValidKey = await _paymentService.ClubHasValidKey(userRole.ClubId);
            var EventResultTypes = from ResultType e in Enum.GetValues(typeof(ResultType))
                                   select new { Id = (int)e, Name = e.ToString() };

            ViewBag.EventResultType = new SelectList(EventResultTypes, "Id", "Name");


            ViewBag.CultureInfo = Thread.CurrentThread.CurrentCulture.Name;
            ViewBag.Languages = new List<CultureInfo> { new("en-US"), new("pt-PT") };

            //ViewBag.NumberOfEnrolls = (await _eventService.GetEnrolls(myEvent.Id)).Count();

            Event eventCopy = new Event
            {
                StartDate = myEvent.StartDate,
                EndDate = myEvent.EndDate,
                EnrollLimitDate = myEvent.EnrollLimitDate,
                IsPublic = myEvent.IsPublic,
                Fee = myEvent.Fee,
                HaveRoute = myEvent.HaveRoute,
                Route = myEvent.Route,
                EventResultType = myEvent.EventResultType,
                MaxEventEnrolls = myEvent.MaxEventEnrolls,
                AddressByPath = myEvent.AddressByPath,
                Location = myEvent.Location,
                LocationId = myEvent.LocationId,
            };

            EventModel eventToEdit = new EventModel
            {
                StartDate = myEvent.StartDate,
                EndDate = myEvent.EndDate,
                EnrollLimitDate = myEvent.EnrollLimitDate,
                IsPublic = myEvent.IsPublic,
                Fee = myEvent.Fee,
                HaveRoute = myEvent.HaveRoute,
                Route = myEvent.Route,
                EventResultType = myEvent.EventResultType,
                MaxEventEnrolls = myEvent.MaxEventEnrolls,
                AddressByPath = myEvent.AddressByPath,
                EventTranslationsName = myEvent.EventTranslations.Where(e => e.Atribute == "Name").ToList(),
                EventTranslationsDetails = myEvent.EventTranslations.Where(e => e.Atribute == "Details").ToList(),
                Location = myEvent.Location,
                LocationId = myEvent.LocationId,
                CreationDate = myEvent.CreationDate,
                EventAux = JsonSerializer.Serialize(eventCopy)
            };

            return View(eventToEdit);
        }

        private async Task UpdateTranslations(ICollection<EventTranslation> eventTranslations, Event actualEvent)
        {
            //update translations
            ICollection<EventTranslation> clubTranslationsFromFrontend = new List<EventTranslation>(eventTranslations);

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

        private bool CheckEnroll(EventModel myEvent)
        {
            //Check if StartDate,EndDate,EventTranslationsName,EventTranslationsDetails,IsPublic,Fee,HaveRoute,Route,EventResultType,MaxEventEnrolls,AddressByPath,LocationString, EventAux is not null
            //then return true else return false
            return (myEvent.StartDate != null && myEvent.EndDate != null && myEvent.EventTranslationsName != null && myEvent.EventTranslationsDetails != null && myEvent.IsPublic != null && myEvent.Fee != null && myEvent.HaveRoute != null && myEvent.EventResultType != null && myEvent.MaxEventEnrolls != null && myEvent.EventAux != null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartDate,EndDate,EventTranslationsName,EventTranslationsDetails,IsPublic,Fee,HaveRoute,Route,EnrollLimitDate,EventResultType,MaxEventEnrolls,AddressByPath,LocationString, EventAux")] EventModel myEvent)
        {
            if (id != myEvent.Id)
            {
                return View("CustomError", "Error_NotFound");
            }

            if (ModelState.IsValid || (CheckEnroll(myEvent) && myEvent.EnrollLimitDate == new DateTime()))
            {
                var eventToUpdate = await _eventService.GetEvent(id);
                if (eventToUpdate == null) return View("CustomError", "Error_NotFound");

                //check if user is staff of the club that owns the event
                var userRole = await _clubService.GetUserRoleInClub(getUserIdFromAuthedUser(), eventToUpdate.ClubId);

                if (!_clubService.IsClubStaff(userRole))
                {
                    return View("CustomError", "Error_Unauthorized");
                }

                Event eventCopy = JsonSerializer.Deserialize<Event>(myEvent.EventAux);
                if (eventCopy == null) return View("CustomError", "Error_NotFound");
                if (myEvent.StartDate == eventCopy.StartDate && myEvent.EndDate == eventCopy.EndDate && (myEvent.EnrollLimitDate == eventCopy.EnrollLimitDate || myEvent.EnrollLimitDate == new DateTime()))
                {

                }
                else if ((myEvent.StartDate < myEvent.CreationDate) || myEvent.EndDate < myEvent.StartDate || myEvent.EnrollLimitDate > myEvent.StartDate)
                {
                    return View("CustomError", "Error_InvalidInput");
                }

                var validKey = await _paymentService.ClubHasValidKey(userRole.ClubId);

                //Quando o eventToUpdate ja tiver uma localização e o myEvent tiver um addressByPath ele vai meter a localização a null e guardar o address
                if (eventToUpdate.LocationId != null && myEvent.Route != null)
                {
                    await _eventService.RemoveEventAddress(eventToUpdate);
                    eventToUpdate.LocationId = null;
                    eventToUpdate.Location = null;
                }
                else
                {

                    Address newLocation = null;
                    Address location = null;
                    if (myEvent.LocationString != null)
                    {
                        location = JsonConvert.DeserializeObject<Address>(myEvent.LocationString);
                    }

                    if (eventToUpdate.LocationId != null && location != null)
                    {
                        //Update Location Address
                        newLocation = await _eventService.UpdateEventAddress((int)eventToUpdate.LocationId, location);
                    }
                    else
                    {
                        //Create Location Address
                        if (location != null)
                        {
                            newLocation = await _eventService.CreateEventAddress(location);
                            eventToUpdate.LocationId = newLocation == null ? null : newLocation.Id;

                        }
                    }
                }

                eventToUpdate.StartDate = myEvent.StartDate;
                eventToUpdate.EndDate = myEvent.EndDate;
                eventToUpdate.IsPublic = myEvent.IsPublic;
                eventToUpdate.Fee = validKey ? myEvent.Fee : 0;
                eventToUpdate.HaveRoute = myEvent.Route != null;
                eventToUpdate.Route = myEvent.Route;
                eventToUpdate.EventResultType = myEvent.EventResultType;
                eventToUpdate.EnrollLimitDate = myEvent.EnrollLimitDate == new DateTime() ? eventCopy.EnrollLimitDate : myEvent.EnrollLimitDate;
                eventToUpdate.MaxEventEnrolls = myEvent.MaxEventEnrolls == 0 ? int.MaxValue : myEvent.MaxEventEnrolls;
                eventToUpdate.AddressByPath = myEvent.AddressByPath;

                await UpdateTranslations(myEvent.EventTranslationsName, eventToUpdate);
                await UpdateTranslations(myEvent.EventTranslationsDetails, eventToUpdate);

                var translations = new List<EventTranslation>(myEvent.EventTranslationsName);
                translations.AddRange(myEvent.EventTranslationsDetails);
                eventToUpdate.EventTranslations = translations;

                var eveUpdated = await _eventService.UpdateEvent(eventToUpdate);

                await _paymentService.UpdateProductEvent(eventToUpdate);

                _notificationService.NotifyEventEdit(eveUpdated);

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

            //check if user is staff of the club that owns the event
            var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());
            if (!_clubService.IsClubStaff(role) || role.ClubId != myEvent.ClubId)
            {
                return View("CustomError", "Error_Unauthorized");
            }

            await _paymentService.UpdateProductEvent(myEvent, true);
            await _eventService.RemoveEventAddress(myEvent);
            await _eventService.DeleteEvent(myEvent);

            _notificationService.NotifyEventDeleted(myEvent);

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

            var userRole = await _clubService.GetUserRoleInClub(userId, eventToEnroll.ClubId);

            //check if event is public
            if (!eventToEnroll.IsPublic && userRole.RoleId == 0)
            {
                return View("CustomError", "Error_Unauthorized");
            }

            //check if user is already enrolled
            if (await _eventService.GetEnroll(id, userId) != null)
            {
                return View("CustomError", "Error_AlreadyEnrolled");
            }

            //Verify number of enrollments on event if do not exceed the limit
            var numberOfEnrollments = await _eventService.GetNumberOfEnrolls(id);
            if (numberOfEnrollments >= eventToEnroll.MaxEventEnrolls)
            {
                return View("CustomError", "Error_MaxNumberOfEnrollments");
            }

            //Check Date
            if (DateTime.Now >= eventToEnroll.EnrollLimitDate)
            {
                return View("CustomError", "Error_LimitDateExceed");
            }

            //create Enrollment
            var enroll = await _eventService.CreateEventEnroll(new EventEnroll
            {
                EventId = id,
                UserId = userId,
                EnrollDate = DateTime.Now,
                EnrollStatus = eventToEnroll.Fee != 0 ? EnrollPaymentStatus.Pending : EnrollPaymentStatus.Valid
            });

            var pay = await _paymentService.CreateEventPayment(enroll);
            var needsPayment = pay != null;
            _notificationService.NotifyEventJoined(enroll, needsPayment);

            if (pay != null) return RedirectToAction("Index", "Payment", new { payId = pay.Id });

            return RedirectToAction(nameof(Details), new { id });
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
            //If fee = 0 is possible to cancel with enrolment paymend valid
            if (myEvent.Fee != 0 && enrollRoRemove.EnrollStatus != EnrollPaymentStatus.Pending) return View("CustomError", "Error_Unauthorized");

            await _eventService.CancelEventEnroll(enrollRoRemove);
            await _paymentService.CancelEventPayment(enrollRoRemove);

            _notificationService.NotifyEventLeft(enrollRoRemove, false);

            return RedirectToAction(nameof(Details), new { id = id });
        }


        public async Task<IActionResult> PathInfoMapBox(int id)
        {
            var ev = await _eventService.GetEvent(id);

            if (ev == null) return View("CustomError", "Error_NotFound");
            if (ev.Route == null) return View("CustomError", "Error_NotFound");
            return View(ev);
        }


        [Authorize]
        public async Task<IActionResult> UpdateEventLocation(int id)
        {
            var myEvent = await _eventService.GetEvent(id);
            if (myEvent == null) return View("CustomError", "Error_NotFound");


            var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());
            if (myEvent.ClubId != role.ClubId || !_clubService.IsClubStaff(role))
            {
                return View("CustomError", "Error_Unauthorized");
            }

            return View(myEvent);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateLocation(int eventId, Address address)
        {
            var userId = getUserIdFromAuthedUser();

            var role = await _userService.GetSelectedRole(userId);
            if (role == null)
                return View("CustomError", "Error_Unauthorized");

            var myEvent = await _eventService.GetEvent(eventId);
            if (myEvent == null) return View("CustomError", "Error_NotFound");

            if (myEvent.ClubId != role.ClubId || !_clubService.IsClubStaff(role))
                return View("CustomError", "Error_Unauthorized");

            if (myEvent.LocationId == null)
            {
                var newAddress = await _eventService.CreateEventAddress(address);
                myEvent.LocationId = newAddress.Id;
                myEvent.AddressByPath = null;
                myEvent.Route = null;
                myEvent.HaveRoute = false;
                await _eventService.UpdateEvent(myEvent);
            }
            else
            {
                await _eventService.UpdateEventAddress((int)myEvent.LocationId, address);
            }

            return Json(new { url = "/Events/Edit/" + myEvent.Id });
        }

        ///Events/GetAllEvents
        public async Task<IActionResult> GetAllEvents()
        {
            var userId = getUserIdFromAuthedUser();

            var events = await _eventService.GetEvents(userId);
            events.OrderBy(e => e.StartDate);

            //For each event create a new object but he go add name of the event in the object
            var eventAux = events.Select(e => new
            {
                Id = e.Id,
                Translate = e.EventTranslations.Where(et => et.Atribute == "Name").Select(e => e.Value).FirstOrDefault(),
                StartDate = e.StartDate,
                ClubName = e.Club.Name

            });


            return Json(eventAux);
        }

        public async Task<IActionResult> Results(int id)
        {
            var myEvent = await _eventService.GetEvent(id);
            if (myEvent == null) return View("CustomError", "Error_NotFound");

            var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());
            if (myEvent.ClubId == role.ClubId && _clubService.IsClubStaff(role))
            {
                ViewBag.IsStaff = true;
            }

            string cultureInfo = Thread.CurrentThread.CurrentCulture.Name;
            myEvent.EventTranslations = myEvent.EventTranslations!.Where(cc => cc.Language == cultureInfo).ToList();

            var results = await _eventService.GetResults(myEvent.Id);
            if (results != null)
            {
                if (myEvent.EventResultType == ResultType.Time)
                {
                    results = results.OrderBy(r => r.Time).ToList();
                }
                else if (myEvent.EventResultType == ResultType.Score)
                {
                    results = results.OrderBy(r => r.Score).ToList();
                }
                else
                {
                    results = results.OrderBy(r => r.Position).ToList();
                }
            }

            myEvent.Results = results;

            return View(myEvent);
        }

        public async Task<IActionResult> AddResult(int id)
        {
            var myEvent = await _eventService.GetEvent(id);
            if (myEvent == null) return View("CustomError", "Error_NotFound");

            var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());
            if (myEvent.ClubId != role.ClubId || !_clubService.IsClubStaff(role))
            {
                return View("CustomError", "Error_Unauthorized");
            }

            //Get all users enrolled in the event and not have result
            var enrolls = await _eventService.GetEnrolls(myEvent.Id);

            if (enrolls != null)
            {
                var usersEnrolled = enrolls.Where(e => e.EnrollStatus == EnrollPaymentStatus.Valid).ToList();

                if (myEvent.EventResultType == ResultType.Time)
                {
                    ViewBag.EventResultType = "Time";
                }
                else if (myEvent.EventResultType == ResultType.Position)
                {
                    ViewBag.EventResultType = "Position";
                    ViewBag.MaxPosition = usersEnrolled.Count;
                }

                var usersEnrolledWithResult = await _eventService.GetResults(myEvent.Id);
                if (usersEnrolledWithResult != null)
                {
                    var usersStrng = usersEnrolledWithResult.Select(er => er.UserId).ToList();

                    usersEnrolled.RemoveAll(u => usersStrng.Contains(u.UserId));
                }

                if (usersEnrolled.Count == 0)
                {
                    return View("CustomError", "Error_NoUsersToResult");
                }

                ViewBag.UsersToResult = new SelectList(usersEnrolled.Select(u => u.User).ToList(), "Id", "FullName");


            }

            return PartialView("_PartialAddResult");

        }

        [HttpPost]
        public async Task<IActionResult> AddResult(int id, [Bind("Id,UserId,Result")] ResultModel userResult)
        {
            if (ModelState.IsValid)
            {
                var myEvent = await _eventService.GetEvent(id);

                if (myEvent == null)
                {
                    return View("CustomError", "Error_NotFound");
                }

                var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());
                if (role.ClubId != myEvent.ClubId || !_clubService.IsClubStaff(role))
                {
                    return View("CustomError", "Error_Unauthorized");
                }

                //Verify type of result
                EventResult resultToCreate = null;
                if (myEvent.EventResultType == ResultType.Time)
                {
                    resultToCreate = new EventResult
                    {
                        EventId = myEvent.Id,
                        UserId = userResult.UserId,
                        Time = Convert.ToDouble(userResult.Result.Replace(".", ","))
                    };
                }
                else if (myEvent.EventResultType == ResultType.Score)
                {
                    resultToCreate = new EventResult
                    {
                        EventId = myEvent.Id,
                        UserId = userResult.UserId,
                        Score = Convert.ToInt16(userResult.Result)
                    };
                }
                else
                {
                    resultToCreate = new EventResult
                    {
                        EventId = myEvent.Id,
                        UserId = userResult.UserId,
                        Position = Convert.ToInt16(userResult.Result)
                    };
                }

                var resultCreated = await _eventService.CreateResult(resultToCreate);

                //Update Result events
                if (myEvent.Results == null)
                {
                    myEvent.Results = new List<EventResult>();
                }
                myEvent.Results.Add(resultCreated);

                await _eventService.UpdateEvent(myEvent);

                return RedirectToAction(nameof(Results), new { id = myEvent.Id });
            }

            return RedirectToAction(nameof(Results), new { id = id });

        }

        [HttpPost]
        public async Task<IActionResult> DeleteResult(string? userId, int? eventId)
        {
            if (userId == null || eventId == null)
            {
                return View("CustomError", "Error_NotFound");
            }

            var role = await _userService.GetSelectedRole(getUserIdFromAuthedUser());
            if (role == null || !_clubService.IsClubStaff(role))
            {
                return View("CustomError", "Error_Unauthorized");
            }

            var resultToDelete = await _eventService.GetResult(userId, (int)eventId);
            if (resultToDelete == null)
                return View("CustomError", "Error_NotFound");

            await _eventService.DeleteResult(resultToDelete);

            return RedirectToAction(nameof(Results), new { id = eventId });
        }

        public class ResultModel
        {
            public string UserId { get; set; }
            public User? User { get; set; }
            public string Result { get; set; }
        }

    }
}
