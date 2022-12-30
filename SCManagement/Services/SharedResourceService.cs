using System.Reflection;
using Microsoft.Extensions.Options;

namespace SCManagement.Services
{
    public class SharedResourceService
    {
        private readonly IStringLocalizer _localizer;
        public SharedResourceService(IStringLocalizerFactory factory)
        {
            var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName!);
            _localizer = factory.Create(nameof(SharedResource), assemblyName.Name!);
        }

        public string Get(string key, string? culture = null)
        {
            if (culture == null) return _localizer[key];

            var specifiedCulture = new CultureInfo(culture);
            CultureInfo.CurrentCulture = specifiedCulture;
            CultureInfo.CurrentUICulture = specifiedCulture;
            var options = Options.Create(new LocalizationOptions { ResourcesPath = "Resources" });
            var factory = new ResourceManagerStringLocalizerFactory(options, new LoggerFactory());
            var localizer = new StringLocalizer<SharedResource>(factory);
            return localizer[key];
        }
    }
}
