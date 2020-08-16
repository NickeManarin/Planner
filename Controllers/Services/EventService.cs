using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Planner.Data;
using Planner.Model;
using Planner.Model.Transient;

namespace Planner.Controllers.Services
{
    public interface IEventService
    {
        Task<IResponse> GetAll();
        Task<IResponse> Create(EventRequest request);
        Task<IResponse> Edit(EventRequest request);
        Task<IResponse> Remove(EventRequest request);
    }

    public class EventService : IEventService
    {
        private readonly DataContext _context;

        public EventService(DataContext context)
        {
            _context = context;
        }


        public async Task<IResponse> GetAll()
        {
            return new EventResponse
            {
                Code = (int)HttpStatusCode.OK,
                Status = (int)StatusCode.OkWithContent,
                Events = await _context.Events.Include(i => i.EventsUsers).ThenInclude(t => t.User).Select(s => new Event
                {
                    Id = s.Id,
                    DueTo = s.DueTo,
                    Name = s.Name,
                    SuggestedValue = s.SuggestedValue,
                    SuggestedValueWithDrinks = s.SuggestedValueWithDrinks,
                    Observation = s.Observation,
                    EventsUsers = s.EventsUsers.Select(e => new Participation
                    {
                        UserId = e.UserId,
                        User = new User
                        {
                            Id = e.User.Id,
                            Name = e.User.Name,
                            WasDeactivated = e.User.WasDeactivated
                        },
                        Contribution = e.Contribution,
                        HasPaid = e.HasPaid,
                        Observation = e.Observation,
                        AddedIn = e.AddedIn,
                        EventId = e.EventId,
                    }).ToList()
                }).ToListAsync()
            };
        }

        public async Task<IResponse> Create(EventRequest request)
        {
            var evnt = await _context.Events.FirstOrDefaultAsync(f => f.Name == request.Name && f.DueTo == request.DueTo);

            //Event already created.
            if (evnt != null)
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.AlreadyInUse
                };

            //One user can only participate once in a event.
            if (request.EventsUsers.GroupBy(x => x.UserId).Any(g => g.Count() > 1))
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.RepeatedParticipant
                };

            await using var tra = await _context.Database.BeginTransactionAsync();

            try
            {
                //Event creation.
                evnt = new Event
                {
                    Name = request.Name,
                    DueTo = request.DueTo,
                    SuggestedValue = request.SuggestedValue,
                    SuggestedValueWithDrinks = request.SuggestedValueWithDrinks,
                    Observation = request.Observation
                };

                await _context.Events.AddAsync(evnt);
                await _context.SaveChangesAsync();

                //Participants.
                foreach (var participation in request.EventsUsers)
                {
                    participation.EventId = evnt.Id;

                    await _context.Participations.AddAsync(new Participation
                    {
                        UserId = participation.UserId,
                        EventId = evnt.Id,
                        Contribution = participation.Contribution,
                        AddedIn = DateTime.UtcNow,
                        HasPaid = participation.HasPaid,
                        Observation = participation.Observation
                    });
                }

                await _context.SaveChangesAsync();
                await tra.CommitAsync();

                return await GetAll();
            }
            catch (Exception e)
            {
                await tra.RollbackAsync();
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IResponse> Edit(EventRequest request)
        {
            var evnt = await _context.Events.Include(i => i.EventsUsers).FirstOrDefaultAsync(f => f.Id == request.Id);

            if (evnt == null)
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.EventMissing
                };

            await using var tra = await _context.Database.BeginTransactionAsync();

            try
            {
                //Event.
                evnt.Name = request.Name;
                evnt.DueTo = request.DueTo;
                evnt.SuggestedValue = request.SuggestedValue;
                evnt.SuggestedValueWithDrinks = request.SuggestedValueWithDrinks;
                evnt.Observation = request.Observation;

                //Removed participations.
                foreach (var part in evnt.EventsUsers)
                {
                    if (!request.EventsUsers.Any(a => a.EventId == part.EventId && a.UserId == part.UserId))
                        _context.Participations.Remove(part);
                }

                //Added or udpdated participations.
                foreach (var part in request.EventsUsers)
                {
                    var found = evnt.EventsUsers.FirstOrDefault(f => f.EventId == part.EventId && f.UserId == part.UserId);

                    if (found != null)
                    {
                        found.HasPaid = part.HasPaid;
                        found.AddedIn = part.AddedIn;
                        found.Contribution = part.Contribution;
                        found.Observation = part.Observation;
                        //_context.Participations.Update(found);
                        continue;
                    }

                    await _context.Participations.AddAsync(new Participation
                    {
                        EventId = evnt.Id,
                        UserId = part.UserId,
                        AddedIn = part.AddedIn,
                        HasPaid = part.HasPaid,
                        Contribution = part.Contribution,
                        Observation = part.Observation
                    });
                }

                //evnt.EventsUsers = request.EventsUsers.Select(s => new Participation
                //{
                //    EventId = s.EventId,
                //    UserId = s.UserId,
                //    AddedIn = s.AddedIn,
                //    HasPaid = s.HasPaid,
                //    Contribution = s.Contribution,
                //    Observation = s.Observation
                //}).ToList();

                //_context.Events.Update(evnt);
                await _context.SaveChangesAsync();
                await tra.CommitAsync();

                return await GetAll();
            }
            catch (Exception e)
            {
                await tra.RollbackAsync();
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IResponse> Remove(EventRequest request)
        {
            var evnt = await _context.Events.FirstOrDefaultAsync(f => f.Id == request.Id);

            if (evnt == null)
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.EventMissing
                };

            await using var tra = await _context.Database.BeginTransactionAsync();

            try
            {
                //Participants.
                foreach (var participation in request.EventsUsers)
                {
                    var part = await _context.Participations.Where(w => w.EventId == evnt.Id && w.UserId == participation.UserId).FirstOrDefaultAsync();

                    if (part != null)
                        _context.Participations.Remove(part);
                }

                _context.Events.Remove(evnt);
                await _context.SaveChangesAsync();
                await tra.CommitAsync();

                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.OK,
                    Status = (int)StatusCode.OkWithNoContent,
                };
            }
            catch (Exception e)
            {
                await tra.RollbackAsync();
                Console.WriteLine(e);
                throw;
            }
        }
    }
}