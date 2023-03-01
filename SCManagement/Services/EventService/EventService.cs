using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Models;

namespace SCManagement.Services.EventService
{
    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Event> CreateEvent(Event myEvent)
        {
            _context.Event.Add(myEvent);
            await _context.SaveChangesAsync();
            return myEvent;
        }

        public async Task DeleteEvent(Event myEvent)
        {
            _context.Event.Remove(myEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<Event?> GetEvent(int eventId)
        {
            return await _context.Event.Include(e => e.Club).Include(e => e.Location).FirstOrDefaultAsync(e => e.Id == eventId);

        }

        public async Task<IEnumerable<Event>> GetPublicEvents()
        {
            return await _context.Event.Where(e => e.IsPublic == true).Include(e => e.Club).ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetClubEvents(int clubId)
        {
            return await _context.Event.Where(e => e.ClubId == clubId && e.IsPublic == false).Include(e => e.Club).ToListAsync();
        }

        public async Task<Event> UpdateEvent(Event myEvent)
        {
            _context.Event.Update(myEvent);
            await _context.SaveChangesAsync();
            return myEvent;
        }

        public async Task<EventEnroll> CreateEventEnroll(EventEnroll enroll)
        {
            _context.EventEnroll.Add(enroll);
            await _context.SaveChangesAsync();
            return enroll;
        }

        public async Task<EventEnroll?> GetEnroll(int eventId, string userId)
        {
            return await _context.EventEnroll.Where(e => e.EventId == eventId && e.UserId == userId).Include(e => e.User).FirstOrDefaultAsync();
        }

        public async Task CancelEventEnroll(EventEnroll enroll)
        {
            _context.EventEnroll.Remove(enroll);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetNumberOfEnrolls(int eventId)
        {
            return await _context.EventEnroll.Where(e => e.EventId == eventId).CountAsync();
        }
        public async Task<IEnumerable<EventEnroll>> GetEnrolls(int eventId)
        {
            return await _context.EventEnroll.Where(e => e.EventId == eventId).Include(e => e.User).ToListAsync();
        }
        public async Task<Address> CreateEventAddress(Address address)
        {
            _context.Address.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }
        public async Task<Address> UpdateEventAddress(int locationId, Address address)
        {
            Address ad = _context.Address.Find(locationId);
            ad.CoordinateX = address.CoordinateX;
            ad.CoordinateY = address.CoordinateY;
            ad.ZipCode = address.ZipCode;
            ad.Street = address.Street;
            ad.City = address.City;
            ad.District = address.District;
            ad.Country = address.Country;

            _context.Address.Update(ad);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task RemoveEventAddress(Event myEvent)
        {
            {
                Address ad = _context.Address.Find(myEvent.LocationId);
                
                if (ad == null) return;
                
                //myEvent.Location = null;
                //myEvent.LocationId = null;
                //_context.Event.Update(myEvent);
                
                _context.Address.Remove(ad);
                await _context.SaveChangesAsync();
            }
        }

    }
    }
    





