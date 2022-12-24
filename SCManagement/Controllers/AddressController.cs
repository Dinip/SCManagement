using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;
using SCManagement.Services.Location;

namespace SCManagement.Controllers
{
    public class AddressController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILocationService _locationService;

        public AddressController(ApplicationDbContext context, ILocationService locationService)
        {
            _context = context;
            _locationService = locationService;
        }

        [HttpPost]
        public async Task<IActionResult> GetCountries()
        {
            ViewBag.Countries = new SelectList(await _locationService.GetCountries(), "Id", "Name");
            return ViewComponent("Address");
        }

        [HttpPost]
        public async Task<IActionResult> GetDistricts(int id)
        {
            ViewBag.Districts = new SelectList(await _locationService.GetDistricts(id), "Id", "Name");
            return ViewComponent("Address");
        }

        [HttpPost]
        public async Task<IActionResult> GetCounties(int id)
        {
            ViewBag.Counties = new SelectList(await _locationService.GetCounties(id), "Id", "Name");
            return ViewComponent("Address");
        }

    }
}
