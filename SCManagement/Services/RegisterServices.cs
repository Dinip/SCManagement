using SCManagement.Services.EmailService;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using SCManagement.Data;
using SCManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using SCManagement.Services.AzureStorageService;
using SCManagement.Middlewares;
using SCManagement.Services.ClubService;

namespace SCManagement.Services
{
    public class RegisterServices
    {
        public static void Register(IConfiguration configuration, IServiceCollection services)
        {
            //to use azure key vault for secrets instead of local secrets.json or appsettings.json
            //disabled in development
            //Configuration.AddAzureKeyVault(new Uri(Configuration["KeyVaultUri"]), new DefaultAzureCredential());

            // Add services to the container.
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            })
              .AddDefaultUI()
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

            #region add support for localization
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[]
                {
                    new CultureInfo("en-US"),
                    new CultureInfo("pt-PT"),
                };
                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
                options.FallBackToParentCultures = true;
                options.FallBackToParentUICultures = true;

                options.RequestCultureProviders = new List<IRequestCultureProvider>
                {
                    new QueryStringRequestCultureProvider(),
                    new CookieRequestCultureProvider()
                };
            });
            services.AddSingleton<SharedResourceService>();
            services.AddScoped<RequestLocalizationCookiesMiddleware>();
            #endregion

            #region register google authentication
            string GoogleId = configuration["GoogleId"];
            string GoogleSecret = configuration["GoogleSecret"];

            if (!string.IsNullOrEmpty(GoogleId) && !string.IsNullOrEmpty(GoogleSecret))
            {
                services.AddAuthentication().AddGoogle(options =>
                {
                    options.ClientId = configuration["GoogleId"];
                    options.ClientSecret = configuration["GoogleSecret"];
                    //options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
                    //options.ClaimActions.MapJsonKey("urn:google:locale", "locale", "string");
                });
            }
            #endregion

            #region register microsoft authentication
            string MicrosoftId = configuration["MicrosoftId"];
            string MicrosoftSecret = configuration["MicrosoftSecret"];

            if (!string.IsNullOrEmpty(MicrosoftId) && !string.IsNullOrEmpty(MicrosoftSecret))
            {
                services.AddAuthentication().AddMicrosoftAccount(options =>
                {
                    options.ClientId = configuration["MicrosoftId"];
                    options.ClientSecret = configuration["MicrosoftSecret"];
                });
            }
            #endregion

            services.AddRazorPages();
            services.AddMvc().AddViewLocalization().AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        return factory.Create(typeof(SharedResource));
                    };
            });
            services.AddControllersWithViews();

            #region email service
            //add email service, in development it uses the local smtp server from mail catcher
            //in production it will use mailgun with the api key stored in the vault
            services.Configure<AuthMessageSenderOptions>(options => options.AuthKey = configuration["MailtrapKey"]);
            services.AddTransient<IEmailSender, EmailSenderMailtrap>();
            //services.AddTransient<IEmailSender, EmailSenderMailgun>();
            #endregion

            services.AddTransient<IAzureStorage, AzureStorage>();
            services.AddTransient<IClubService, SCManagement.Services.ClubService.ClubService>();
        }
    }
}
