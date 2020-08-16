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
    public class ParticipationController : ControllerBase
    {
        private readonly IParticipationService _participationService;
        private readonly AppSettings _appSettings;

        public ParticipationController(IParticipationService participationService, IOptions<AppSettings> appSettings)
        {
            _participationService = participationService;
            _appSettings = appSettings.Value;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll(long eventId)
        {
            try
            {
                var users = await _participationService.GetAll(eventId);

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
        public async Task<IActionResult> Create([FromBody] ParticipationRequest request)
        {
            try
            {
                var response = await _participationService.Create(request);

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
        public async Task<IActionResult> Edit([FromBody] ParticipationRequest request)
        {
            try
            {
                var response = await _participationService.Edit(request);

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
        public async Task<IActionResult> Remove([FromBody] ParticipationRequest request)
        {
            try
            {
                var response = await _participationService.Remove(request);

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