using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Models;
using SCManagement.Services.UserService;

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


        /// <summary>
        /// Updates the selected role for the user
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
    }
}
