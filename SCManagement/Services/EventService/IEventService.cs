﻿using SCManagement.Models;

namespace SCManagement.Services.EventService
{
    public interface IEventService
    {
        public Task<Event?> GetEvent(int eventId);
        public Task<IEnumerable<Event>> GetPublicEvents();
        public Task<IEnumerable<Event>> GetClubEvents(int clubId);
        public Task<Event> CreateEvent(Event myEvent);
        public Task<Event> UpdateEvent(Event myEvent);
        public Task DeleteEvent(Event myEvent);
        public Task<EventEnroll> CreateEventEnroll(EventEnroll enroll);
        public Task<EventEnroll?> GetEnroll(int eventId, string userId);
        public Task CancelEventEnroll(EventEnroll enroll);
        public Task<int> GetNumberOfEnrolls(int eventId);
        public Task<IEnumerable<EventEnroll>> GetEnrolls(int eventId);
        public Task<Address> CreateEventAddress(Address address);
        public Task<Address> UpdateEventAddress(int locationId, Address address);

    }
}
