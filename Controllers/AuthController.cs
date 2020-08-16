using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planner.Controllers.Services;
using Planner.Model.Transient;

namespace Planner.Controllers
{
    [Authorize(Roles = "Admin, User")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Touch()
        {
            try
            {
                return StatusCode(200, "API is running...");
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

        [AllowAnonymous]
        [HttpPost("isavailable")]
        public async Task<IActionResult> IsAvailable([FromBody] AuthorizationRequest request)
        {
            try
            {
                //Checks if the name has no user created.
                var response = await _authService.IsAvailable(request.Email, request.Password);

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

        [AllowAnonymous]
        [HttpPost("verification")]
        public async Task<IActionResult> Verification([FromBody] VerificationRequest request)
        {
            try
            {
                //Generates or checks the verification code.
                var response = await _authService.Verification(request.Name, request.Code);

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

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] AuthorizationRequest request)
        {
            try
            {
                Console.WriteLine($"SignUp request from {request.Email}");

                //Tries to create an user entry.
                var response = await _authService.SignUp(request.Email, request.Name, request.Password);

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

        [AllowAnonymous]
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] AuthorizationRequest request)
        {
            try
            {
                Console.WriteLine($"SignIn request from {request.Email}");

                //Logs in, returning a access token and a refresh token.
                var response = await _authService.SignIn(request.Email, request.Password);

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

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshRequest request)
        {
            try
            {
                //If the client detects that its access token is expired, it should call this method to recieve a new one.
                var response = await _authService.RefreshAccessToken(request);

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


        [AllowAnonymous]
        [HttpPost("password/forgotten")]
        public async Task<IActionResult> PasswordForgotten([FromBody] RefreshRequest request)
        {
            try
            {
                //TODO If the user forgot its password, I should send a link via email to open a page to type a new password.
                var response = await _authService.RefreshAccessToken(request);

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

        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshRequest request)
        {
            try
            {
                //Tries to create an user entry.
                var response = await _authService.RevokeRefreshToken(request);

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

        [HttpPut("password/change")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeRequest request)
        {
            try
            {
                var issuedAt = AuthService.TokenIssueDateTime(Request.Headers["Authorization"]);

                //Tries to create an user entry.
                var response = await _authService.ChangePassword(request, issuedAt);

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
        
        [HttpPost("remove")]
        public async Task<IActionResult> RemoveAccount([FromBody] RemovalRequest request)
        {
            try
            {
                var issuedAt = AuthService.TokenIssueDateTime(Request.Headers["Authorization"]);

                //Tries to remove an user entry and revoke any related token.
                var response = await _authService.RemoveAccount(request, issuedAt);

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