using SCManagement.Models;

namespace SCManagement.Services.Location
{
    public interface ILocationService
    {
        public Task<IEnumerable<Country>> GetCountries();
        public Task<IEnumerable<District>> GetDistricts(int countryId);
        public Task<IEnumerable<County>> GetCounties(int districtId);
        public Task<IEnumerable<County>> SearchCountiesName(string name);

        public Task<Address> CreateAddress(double CoordinateX, double CoordinateY, string? ZipCode, string Street, string City, string District, string Country);

        public Task<IEnumerable<Address>> GetAddress();
    }
}
