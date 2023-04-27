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

        /// <summary>
        /// User controller constructor, injects all the services needed
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="userManager"></param>
        public UserController(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        /// <summary>
        /// Get id of the user that makes the request
        /// </summary>
        /// <returns></returns>
        private string getUserIdFromAuthedUser()
        {
            return _userManager.GetUserId(User);
        }

        /// <summary>
        /// Updates the user selected role (club function)
        /// Used in the navbar dropdown
        /// </summary>
        /// <param name="usersClubRoleId"></param>
        /// <returns></returns>
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
                { "Team", new List<Notification>() },
                { "TrainingPlan", new List<Notification>() },
                { "MealPlan", new List<Notification>() },
                { "Goal", new List<Notification>() },
            };
        }

        /// <summary>
        /// Gets the current user notifications settings (on or off) grouped
        /// by their category
        /// </summary>
        /// <returns></returns>
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
                else if (!type.Contains("Subscription") && !type.Contains("Payment"))
                {
                    settings.Notifications["General"].Add(notification);
                }
            }

            return View(settings);
        }

        /// <summary>
        /// Updates the user notifications settings (on or off)
        /// </summary>
        /// <param name="notifications"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNotificationsSettings(Dictionary<int, Notification> notifications)
        {
            if (!ModelState.IsValid) return View("CustomError", "Error_Unauthorized");

            string userId = getUserIdFromAuthedUser();
            List<Notification> oldNotifications = (await _userService.GetUserWithNotifications(userId)).Notifications.ToList();
            var newNotifications = notifications.Values.ToList();

            oldNotifications.ForEach(notification =>
            {
                var v = newNotifications.FirstOrDefault(f => f.Type == notification.Type);
                if (v != null)
                {
                    notification.IsEnabled = v.IsEnabled;
                }
            });

            await _userService.UpdateNotifications(oldNotifications);
            
            return RedirectToAction("Index", "Home");
        }
    }
}
