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
    public class AddressTController : Controller
    {
        private readonly ILocationService _locationService;

        public AddressTController( ILocationService locationService)
        {
            _locationService = locationService;
        }


        //[HttpPost]
        //public async Task<Address> Create()
        //{
        //    var countries = await _locationService.GetCountries();
        //    var districts = await _locationService.GetDistricts(1);
        //    var counties = await _locationService.GetCounties(1);

        //  var address = new Address
        //  {
        //    Countries = countries,
        //    Districts = districts,
        //    Counties = counties
        //  };
        //  return View(addressView);
        //}

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

        

    }
}
