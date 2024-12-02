using Application.Command.UserCommands;
using Application.Command.ViewModels;
using Application.Query.UserQueries;
using Application.Query.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Rest.V1.RequestModels;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Service.Rest.V1.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        [SwaggerOperation("User login")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Invalid username or password")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Login successful", typeof(LoginResult))]
        public async Task<ActionResult<LoginResult>> Login([FromBody] LoginModel request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _mediator.Send(new LoginCommand(request.UserName, request.Password));

            if (!result.Success)
            {
                return Unauthorized(result.ErrorMessage);
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("CreateUser")]
        [SwaggerOperation("Create a new user")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "User created successfully", typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        public async Task<ActionResult<bool>> CreateUser([FromBody] CreateUserModel request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _mediator.Send(new CreateUserCommand(request.UserName, request.Password, request.Role, request.FullName));

                if (result)
                    return Ok(true);

                return BadRequest("User creation failed.");
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        [Authorize]
        [Route("{username}")]
        [SwaggerOperation("Get user by username")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "User not found")]
        [SwaggerResponse((int)HttpStatusCode.OK, "User retrieved successfully", typeof(UserViewModel))]
        public async Task<ActionResult<UserViewModel>> GetUserByUserName([FromRoute] string username)
        {
            var result = await _mediator.Send(new GetUserByUserNameQuery(username));

            if (result == null)
            {
                return NotFound("User not found.");
            }

            return Ok(result);
        }
    }
}
