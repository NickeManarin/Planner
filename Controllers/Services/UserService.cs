using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Planner.Data;
using Planner.Model.Transient;

namespace Planner.Controllers.Services
{
    public interface IUserService
    {
        Task<IResponse> GetAll();
    }

    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<IResponse> GetAll()
        {
            return new UsersResponse
            {
                Users = await _context.Users.Where(w => !w.WasDeactivated).Select(s => new UserResponse { Id = s.Id, Name = s.Name }).ToListAsync()
            };
        }
    }
}