﻿using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SCManagement.Data;
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSelectedRole(int usersClubRoleId)
        {
            User user = await _userManager.GetUserAsync(User);

            //check if user has that role (prevent injection)
            var hasRole = (await _userService.GetUserRoles(user.Id)).Any(x => x.Id == usersClubRoleId);
            if (hasRole)
            {
                user.SelectedUserRoleClubId = usersClubRoleId;
                await _userService.UpdateUser(user);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
