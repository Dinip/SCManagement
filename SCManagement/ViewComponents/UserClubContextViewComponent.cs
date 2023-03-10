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
        private readonly IStringLocalizer<SharedResource> _localizer;

        public UserClubContext(IUserService userService, UserManager<User> userManager, IStringLocalizer<SharedResource> localizer)
        {
            _userService = userService;
            _userManager = userManager;
            _localizer = localizer;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            var user = await _userService.GetUserWithRoles(userId);
            if (user == null) return View("UserClubRoles");

            //check if user has any role and if has, check if has hany that is different then partner
            if (user.UsersRoleClub == null || !user.UsersRoleClub.Any(r => r.RoleId != 10)) return View("UserClubRoles");

            //remove partner roles from list
            var rolesWithNames = user.UsersRoleClub.Where(r => r.RoleId != 10).Select(s => new { Id = s.Id, Name = $"{s.Club.Name} ({_localizer[s.Role.RoleName]})" });

            //check if the user has a selected role and if not, set to first one that has available
            //and update the user on db
            if (!user.UsersRoleClub.Any(f => f.Selected))
            {
                await _userService.UpdateSelectedRole(userId, user.UsersRoleClub.First().Id);
            }
            ViewBag.Roles = new SelectList(rolesWithNames, "Id", "Name", user.UsersRoleClub.First(f => f.Selected).Id);
            return View("UserClubRoles");
        }
    }
}
