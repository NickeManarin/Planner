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
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly AppSettings _appSettings;

        public EventController(IEventService eventService, IOptions<AppSettings> appSettings)
        {
            _eventService = eventService;
            _appSettings = appSettings.Value;
        }

        [Authorize(Roles = "Admin, User")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _eventService.GetAll();

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

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] EventRequest request)
        {
            try
            {
                var response = await _eventService.Create(request);

                return StatusCode(response.Code ?? 200, response);
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

        [Authorize(Roles = "Admin")]
        [HttpPost("edit")]
        public async Task<IActionResult> Edit([FromBody] EventRequest request)
        {
            try
            {
                var response = await _eventService.Edit(request);

                return StatusCode(response.Code ?? 200, response);
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

        [Authorize(Roles = "Admin")]
        [HttpPost("remove")]
        public async Task<IActionResult> Remove([FromBody] EventRequest request)
        {
            try
            {
                var response = await _eventService.Remove(request);

                return StatusCode(response.Code ?? 200, response);
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