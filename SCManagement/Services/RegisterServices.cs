using SCManagement.Services.EmailService;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using SCManagement.Data;
using SCManagement.Models;
using Microsoft.EntityFrameworkCore;
using SCManagement.Services.AzureStorageService;
using SCManagement.Middlewares;
using SCManagement.Services.ClubService;
using SCManagement.Services.UserService;
using SCManagement.Services.TeamService;
using Azure.Identity;
using SCManagement.Services.PaymentService;
using SCManagement.Services.EventService;
using SCManagement.Services.TranslationService;
using SCManagement.Services.CronJobService;
using SCManagement.Services.StatisticsService;
using SCManagement.Services.PlansService;

namespace SCManagement.Services
{
    public class RegisterServices
    {
        public static void Register(ConfigurationManager configuration, IServiceCollection services)
        {
            //to use azure key vault for secrets instead of local secrets.json or appsettings.json
            //disabled in development
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (env == "Production")
            {
                configuration.AddAzureKeyVault(new Uri(configuration["KeyVaultUri"]), new DefaultAzureCredential());
            }

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
              // localize identity error messages
              .AddErrorDescriber<LocalizedIdentityErrorDescriber>()
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

            #region add support for localization
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var pt = new CultureInfo("pt-PT");
                var en = new CultureInfo("en-US");
                en.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

                var supportedCultures = new[] { pt, en };
                options.DefaultRequestCulture = new RequestCulture(en);
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

            if (env == "Production")
            {
                services.Configure<AuthMessageSenderOptions>(options => options.AuthKey = configuration["MailgunKey"]);
                services.AddTransient<IEmailSender, EmailSenderMailgun>();
            }
            else
            {
                services.Configure<AuthMessageSenderOptions>(options => options.AuthKey = configuration["MailtrapKey"]);
                services.AddTransient<IEmailSender, EmailSenderMailtrap>();
            }
            #endregion

            #region internal services
            services.AddTransient<IAzureStorage, AzureStorage>();
            services.AddTransient<IUserService, SCManagement.Services.UserService.UserService>();
            services.AddTransient<IClubService, SCManagement.Services.ClubService.ClubService>();
            services.AddTransient<ITeamService, SCManagement.Services.TeamService.TeamService>();
            services.AddTransient<IPaymentService, SCManagement.Services.PaymentService.PaymentService>();
            services.AddTransient<IEventService, SCManagement.Services.EventService.EventService>();
            services.AddTransient<ITranslationService, SCManagement.Services.TranslationService.TranslationService>();
            services.AddTransient<IPlanService, SCManagement.Services.PlansService.PlanService>();
            services.AddTransient<IStatisticsService, SCManagement.Services.StatisticsService.StatisticsService>();         
            services.AddScoped<ApplicationContextService, ApplicationContextService>();
            services.AddScoped<ClubMiddleware>();
            #endregion

            #region cronjobs
            //daily checker @ 1 min past midnight
            services.AddCronJob<DailySubscriptionChecker>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Utc;
                c.CronExpression = @"1 0 * * *";
            });

            // daily suspender @ 10 min past midnight
            services.AddCronJob<DailySubscriptionSuspender>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Utc;
                c.CronExpression = @"10 0 * * *";
            });

            //hourly checker and remover @ 5 min of that hour
            services.AddCronJob<HourlyEventCheckerRemover>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Utc;
                c.CronExpression = @"5 * * * *";
            });

            // daily statistics generator @ 30 min past midnight
            services.AddCronJob<DailyStatisticsGenerator>(c =>
            {
                c.TimeZoneInfo = TimeZoneInfo.Utc;
                c.CronExpression = @"30 0 * * *";
            });
            #endregion
        }
    }
}
