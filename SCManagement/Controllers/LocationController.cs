using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SCManagement.Models;
using SCManagement.Services.Location;

namespace SCManagement.Controllers
{
    [Authorize]
    public class LocationController : Controller
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpPost]
        public async Task<IEnumerable<Country>> GetCountries()
        {
            return await _locationService.GetCountries();
        }

        [HttpPost]
        public async Task<IEnumerable<District>> GetDistricts(int id)
        {
            return await _locationService.GetDistricts(id);
        }

        [HttpPost]
        public async Task<IEnumerable<County>> GetCounties(int id)
        {
            return await _locationService.GetCounties(id);
        }

        [HttpPost]
        public async Task<IEnumerable<County>> Search(string id)
        {
            return await _locationService.SearchCountiesName(id);
        }
    }
}
