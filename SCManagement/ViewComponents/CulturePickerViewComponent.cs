using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SCManagement.Models;

namespace SCManagement.ViewComponents
{
    public class CulturePicker : ViewComponent
    {
        private readonly IOptions<RequestLocalizationOptions> _localizationOptions;


        public CulturePicker(
            IOptions<RequestLocalizationOptions> localizationOptions)
        {
            _localizationOptions = localizationOptions;
        }


        public IViewComponentResult Invoke()
        {
            var cultureFeature = HttpContext.Features.Get<IRequestCultureFeature>();
            var model = new CulturePickerModel
            {
                SupportedCultures = _localizationOptions.Value.SupportedUICultures.ToList(),
                CurrentUICulture = cultureFeature.RequestCulture.UICulture
            };
            return View(model);
        }
    }

    public class CulturePickerModel
    {
        public CultureInfo CurrentUICulture { get; set; }
        public List<CultureInfo> SupportedCultures { get; set; }

        public string ToFlagEmoji(string country)
        {
            country = country.Split('-').LastOrDefault();

            if (country == null)
                return "⁉️️";

            return string.Concat(
                country
                .ToUpper()
                .Select(x => char.ConvertFromUtf32(x + 0x1F1A5))
            );
        }
    }
}
