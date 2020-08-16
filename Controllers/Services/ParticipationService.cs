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
    public interface IParticipationService
    {
        Task<IResponse> GetAll(long eventId);
        Task<IResponse> Create(ParticipationRequest request);
        Task<IResponse> Edit(ParticipationRequest request);
        Task<IResponse> Remove(ParticipationRequest request);
    }

    public class ParticipationService : IParticipationService
    {
        private readonly DataContext _context;

        public ParticipationService(DataContext context)
        {
            _context = context;
        }


        public async Task<IResponse> GetAll(long eventId)
        {
            return new ParticipationResponse
            {
                Participations = await _context.Participations.Where(w => w.EventId == eventId).ToListAsync()
            };
        }

        public async Task<IResponse> Create(ParticipationRequest request)
        {
            var part = await _context.Participations.FirstOrDefaultAsync(f => f.UserId == request.UserId && f.EventId == request.EventId);

            if (part != null)
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.RepeatedParticipant
                };

            await using var tra = await _context.Database.BeginTransactionAsync();

            try
            {
                part = new Participation
                {
                    AddedIn = request.AddedIn,
                    Contribution = request.Contribution,
                    HasPaid = request.HasPaid,
                    Observation = request.Observation
                };

                await _context.Participations.AddAsync(part);
                await _context.SaveChangesAsync();
                await tra.CommitAsync();

                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.OK,
                    Status = (int)StatusCode.OkWithNoContent
                };
            }
            catch (Exception e)
            {
                await tra.RollbackAsync();
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IResponse> Edit(ParticipationRequest request)
        {
            var part = await _context.Participations.FirstOrDefaultAsync(f => f.UserId == request.UserId && f.EventId == request.EventId);

            if (part == null)
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.ParticipationMissing
                };

            await using var tra = await _context.Database.BeginTransactionAsync();

            try
            {
                part.AddedIn = request.AddedIn;
                part.Contribution = request.Contribution;
                part.HasPaid = request.HasPaid;
                part.Observation = request.Observation;

                _context.Participations.Update(part);
                await _context.SaveChangesAsync();
                await tra.CommitAsync();

                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.OK,
                    Status = (int)StatusCode.OkWithNoContent
                };
            }
            catch (Exception e)
            {
                await tra.RollbackAsync();
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<IResponse> Remove(ParticipationRequest request)
        {
            var part = await _context.Participations.FirstOrDefaultAsync(f => f.UserId == request.UserId && f.EventId == request.EventId);

            if (part == null)
                return new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)StatusCode.ParticipationMissing
                };

            await using var tra = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Participations.Remove(part);
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