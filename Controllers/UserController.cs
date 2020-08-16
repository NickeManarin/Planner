using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Planner.Controllers.Services;
using Planner.Model.Transient;

namespace Planner.Controllers
{
    [Authorize(Roles = "Admin, User")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly AppSettings _appSettings;

        public UserController(IUserService userService, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _appSettings = appSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.GetAll();

                return StatusCode(users.Code ?? 200, users);
            }
            catch (Exception)
            {
                return BadRequest(new GenericResponse
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Status = (int)Model.Transient.StatusCode.UnknowException,
                    Message = "Generic error in request."
                });
            }
        }
    }
}