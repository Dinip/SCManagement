using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SCManagement.Data;
using SCManagement.Data.Migrations;
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
            return await _context.Event.Include(e => e.Club).Include(e => e.EventTranslations).FirstOrDefaultAsync(e => e.Id == eventId);
        }
        
        public async Task<IEnumerable<Event>> GetEvents(string? userId)
        {
            string cultureInfo = Thread.CurrentThread.CurrentCulture.Name;

            if (userId == null)
            {
                return await _context.Event
                .Where(e => e.IsPublic == true)
                .Include(e => e.Club)
                .Include(e => e.EventTranslations)
                .Select(e =>
                new Event
                {
                    Id = e.Id,
                    ClubId = e.ClubId,
                    Club = e.Club,
                    EventTranslations = e.EventTranslations!.Where(et => et.Language == cultureInfo).ToList(),
                    IsPublic = e.IsPublic,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                })
                .ToListAsync();
            }
            else
            {
                var clubsIds = _context.UsersRoleClub.Where(u => u.UserId == userId).Select(u => u.ClubId);
                return await _context.Event
                .Where(e => clubsIds.Contains(e.ClubId) || e.IsPublic == true)
                .Include(e => e.Club)
                .Include(e => e.EventTranslations)
                .Select(e =>
                new Event
                {
                    Id = e.Id,
                    ClubId = e.ClubId,
                    Club = e.Club,
                    EventTranslations = e.EventTranslations!.Where(et => et.Language == cultureInfo).ToList(),
                    IsPublic = e.IsPublic,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                })
                .ToListAsync();
            }
        }

        public async Task<IEnumerable<Event>> GetClubEvents(int clubId)
        {
            string cultureInfo = Thread.CurrentThread.CurrentCulture.Name;
            
            return await _context.Event
                .Where(e => e.ClubId == clubId && e.IsPublic == false)
                .Include(e => e.Club)
                .Include(e => e.EventTranslations)
                .Select(e =>
                new Event
                {
                    Id = e.Id,
                    ClubId = e.ClubId,
                    Club = e.Club,
                    EventTranslations = e.EventTranslations!.Where(et => et.Language == cultureInfo).ToList(),
                    IsPublic = e.IsPublic,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                }).ToListAsync();
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

    }
    }
    





