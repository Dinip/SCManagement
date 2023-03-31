using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Models;
using SCManagement.Services.PaymentService.Models;
using SCManagement.Services.UserService;
using static SCManagement.Models.Notification;

namespace SCManagement.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public UserController(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        private string getUserIdFromAuthedUser()
        {
            return _userManager.GetUserId(User);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSelectedRole(int usersClubRoleId)
        {
            string userId = getUserIdFromAuthedUser();
            User user = await _userService.GetUserWithRoles(userId);

            if (user.UsersRoleClub == null) return RedirectToAction("Index", "Home");

            //check if user has that role (prevent injection)
            var hasRole = user.UsersRoleClub.Any(x => x.Id == usersClubRoleId);
            if (hasRole)
            {
                await _userService.UpdateSelectedRole(userId, usersClubRoleId);
            }
            return RedirectToAction("Index", "MyClub");
        }
        public class EditNotificationsSettings
        {
            public Dictionary<string, ICollection<Notification>> Notifications { get; set; }
            = new Dictionary<string, ICollection<Notification>>()
            {
                { "General", new List<Notification>() },
                { "Event", new List<Notification>() },
                { "Club", new List<Notification>() },
                { "Team", new List<Notification>() },
                { "TrainingPlan", new List<Notification>() },
                { "MealPlan", new List<Notification>() },
                { "Goal", new List<Notification>() },
                { "Payment", new List<Notification>() },
                { "Subscription", new List<Notification>() }
            };
        }

        public async Task<IActionResult> UpdateNotificationsSettings()
        {
            string userId = getUserIdFromAuthedUser();
            var notifications = (await _userService.GetUserWithNotifications(userId)).Notifications;

            EditNotificationsSettings settings = new EditNotificationsSettings();
            foreach (Notification notification in notifications)
            {
                string type = notification.Type.ToString();
                string key = settings.Notifications.Keys.FirstOrDefault(k => type.Contains(k));
                if (key != null)
                {
                    settings.Notifications[key].Add(notification);
                }
                else
                {
                    settings.Notifications["General"].Add(notification);
                }
            }

            return View(settings);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNotificationsSettings(Dictionary<int, Notification> notifications)
        {
            if (!ModelState.IsValid) return View("CustomError", "Error_NotFound");

            List<Notification> notificationsToUpdate = new List<Notification>();
            notificationsToUpdate.AddRange(notifications.Values.ToList());
            await _userService.UpdateNotifications(notificationsToUpdate);
            
            return RedirectToAction("Index", "Home");
        }
    }
}
