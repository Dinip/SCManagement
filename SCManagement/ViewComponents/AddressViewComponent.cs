using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SCManagement.Models;
using SCManagement.Services.Location;

namespace SCManagement.ViewComponents
{
    public class AddressViewComponent : ViewComponent
    {
        private readonly ILocationService _locationService;

        public AddressViewComponent(ILocationService locationService)
        {
            _locationService = locationService;
        }
        public async Task<IViewComponentResult> InvokeAsync(AddressComponent? addressComponent)
        {
            var countries = await _locationService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Id", "Name");

            var district = await _locationService.GetDistricts(countries.First().Id);
            ViewBag.Districts = new SelectList(district, "Id", "Name");
            
            ViewBag.Counties = new SelectList(await _locationService.GetCounties(district.First().Id), "Id", "Name");

            if (addressComponent == null)
            {
                return View("Address");
            }
                return View("Address", addressComponent);
        }
    }
}
