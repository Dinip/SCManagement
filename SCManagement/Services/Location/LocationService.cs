using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;

namespace SCManagement.Services.Location
{
    public class LocationService : ILocationService
    {
        private readonly ApplicationDbContext _context;
        public LocationService(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<Country>> GetCountries()
        {
            return await _context.Countries.Include(c => c.Districts).ToListAsync();
        }
        
        public async Task<IEnumerable<District>> GetDistricts(int countryId)
        {
            return await _context.Districts.Where(d => d.CountryId == countryId).ToListAsync();
        }
        
        public async Task<IEnumerable<County>> GetCounties(int id)
        {
            return await _context.Counties.Where(c => c.DistrictId == id).ToListAsync();
        }
    }
}
