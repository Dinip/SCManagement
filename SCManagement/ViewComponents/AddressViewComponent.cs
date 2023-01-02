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
        public async Task<IViewComponentResult> InvokeAsync(Address address)
        {
            return View("LocationSearch", address);
        }
    }
}
