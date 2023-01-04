using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SCManagement.Models;
using SCManagement.Services.UserService;

namespace SCManagement.ViewComponents
{
    public class UserClubContext : ViewComponent
    {
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public UserClubContext(IUserService userService, UserManager<User> userManager)
        {
            _userService = userService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null) return View("UserClubRoles");

            var roles = await _userService.GetUserRoles(user.Id);
            if (roles != null && roles.Count() != 0)
            {
                var rolesWithNames = roles.Select(s => new { Id = s.Id, Name = $"{s.Club.Name} ({s.Role.RoleName})" });
                ViewBag.Roles = new SelectList(rolesWithNames, "Id", "Name", user.SelectedUserRoleClubId);
                return View("UserClubRoles");
            }

            return View("UserClubRoles");
        }
    }
}
