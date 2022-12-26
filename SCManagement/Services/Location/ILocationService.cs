﻿using SCManagement.Models;

namespace SCManagement.Services.Location
{
    public interface ILocationService
    {
        public Task<AddressComponent> Address(AddressComponent addressComponent);
        public Task<IEnumerable<Country>> GetCountries();
        public Task<IEnumerable<District>> GetDistricts(int countryId);
        public Task<IEnumerable<County>> GetCounties(int districtId);
    }

}