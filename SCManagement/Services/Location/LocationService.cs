using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;
using Unidecode.NET;

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
            return await _context.Country.Include(c => c.Districts).ToListAsync();
        }

        public async Task<IEnumerable<District>> GetDistricts(int countryId)
        {
            return await _context.District.Where(d => d.CountryId == countryId).ToListAsync();
        }

        public async Task<IEnumerable<County>> GetCounties(int id)
        {
            return await _context.County.Where(c => c.DistrictId == id).ToListAsync();
        }

        public async Task<IEnumerable<County>> SearchCountiesName(string name)
        {
            string normalizedName = name.Unidecode();
            return await _context.County
                .Where(c => c.NormalizedName.Contains(normalizedName))
                .OrderBy(o => o.Name)
                .Take(5)
                .Include(c => c.District)
                .ThenInclude(d => d.Country)
                .Select(s => new County { Id = s.Id, Name = $"{s.Name}, {s.District!.Name}, {s.District.Country!.Name}" })
                .ToListAsync();
        }
    }
}
