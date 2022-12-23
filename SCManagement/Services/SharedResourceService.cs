using System.Reflection;

namespace SCManagement.Services
{
    public class SharedResourceService
    {
        private readonly IStringLocalizer localizer;
        public SharedResourceService(IStringLocalizerFactory factory)
        {
            var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName!);
            localizer = factory.Create(nameof(SharedResource), assemblyName.Name!);
        }

        public string Get(string key)
        {
            return localizer[key];
        }
    }
}
