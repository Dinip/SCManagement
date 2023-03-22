using Microsoft.AspNetCore.Identity;
using SCManagement.Models;
using SCManagement.Services;
using SCManagement.Services.ClubService;
using SCManagement.Services.UserService;

namespace SCManagement.Middlewares
{
    public static class ClubMiddlewareExtensions
    {
        public static IApplicationBuilder UseClubMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ClubMiddleware>();
            return app;
        }
    }

    public class ClubMiddleware : IMiddleware
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;
        private readonly IClubService _clubService;
        private readonly ApplicationContextService _applicationContextService;


        public ClubMiddleware(UserManager<User> userManager,
            IUserService userService,
            IClubService clubService,
            ApplicationContextService applicationContextService)
        {
            _userManager = userManager;
            _userService = userService;
            _clubService = clubService;
            _applicationContextService = applicationContextService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            //this MW executes after [Authorize], so it is sure
            //that a user is authed at this point
            var userId = _userManager.GetUserId(context.User)!;

            _applicationContextService.UserId = userId;

            var role = await _userService.GetSelectedRole(userId);
            _applicationContextService.UserRole = role;

            if (role.ClubId != 0)
            {
                var status = await _clubService.GetClubStatus(role.ClubId);
                _applicationContextService.ClubStatus = status;

                if (status != ClubStatus.Active)
                {
                    context.Response.Redirect("/MyClub/Unavailable", false);
                }
            }
            await next(context);
        }
    }
}
