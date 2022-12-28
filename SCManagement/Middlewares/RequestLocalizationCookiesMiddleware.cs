using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using SCManagement.Models;

namespace SCManagement.Middlewares
{
    public static class RequestLocalizationCookiesMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLocalizationCookies(this IApplicationBuilder app)
        {
            app.UseMiddleware<RequestLocalizationCookiesMiddleware>();
            return app;
        }
    }

    public class RequestLocalizationCookiesMiddleware : IMiddleware
    {
        public CookieRequestCultureProvider Provider { get; }
        private readonly UserManager<User> _userManager;

        public RequestLocalizationCookiesMiddleware(
            IOptions<RequestLocalizationOptions> requestLocalizationOptions,
            UserManager<User> userManager
            )
        {
            Provider = requestLocalizationOptions
                    .Value
                    .RequestCultureProviders
                    .Where(x => x is CookieRequestCultureProvider)
                    .Cast<CookieRequestCultureProvider>()
                    .FirstOrDefault();

            _userManager = userManager;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (Provider == null)
            {
                await next(context);
                return;
            };

            var feature = context.Features.Get<IRequestCultureFeature>();

            if (feature == null)
            {
                await next(context);
                return;
            };

            var oldCultureCookieValue = context.Request.Cookies.FirstOrDefault(f => f.Key == ".AspNetCore.Culture");

            //check if lang cookie is not set and if so, check if the user is logged in and to set the cookie to the user's lang
            if (oldCultureCookieValue.Value == null)
            {
                var user = await _userManager.GetUserAsync(context.User);
                if (user != null)
                {
                    context.Response
                    .Cookies
                    .Append(
                        Provider!.CookieName,
                        string.Join("|", $"c={user.Language}", $"uic={user.Language}"),
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddMonths(6)
                        }
                    );
                    await next(context);
                    return;
                }

                // default culture              
                context.Response
                    .Cookies
                    .Append(
                        Provider.CookieName,
                        CookieRequestCultureProvider.MakeCookieValue(feature.RequestCulture),
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddMonths(6)
                        }
                    );

                await next(context);
                return;
            }

            //get values for old culture from cookie (c = pt - PT; uic = pt - PT)
            var oldCultureName = oldCultureCookieValue.Value!.Split("|")?[0].Split("=")[1];
            var oldUICultureName = oldCultureCookieValue.Value!.Split("|")?[1].Split("=")[1];

            ////check if any of those are different from the new culture
            if (oldCultureName != feature.RequestCulture.Culture.Name || oldUICultureName != feature.RequestCulture.UICulture.Name)
            {
                //if they are different, and a user is logged in, update that user lang in db
                var user = await _userManager.GetUserAsync(context.User);
                if (user != null)
                {
                    user.Language = feature.RequestCulture.Culture.Name;
                    await _userManager.UpdateAsync(user);
                }
            }

            // remember culture across request                  
            context.Response
                .Cookies
                .Append(
                    Provider.CookieName,
                    CookieRequestCultureProvider.MakeCookieValue(feature.RequestCulture),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddMonths(6)
                    }
                );

            await next(context);
        }
    }
}
