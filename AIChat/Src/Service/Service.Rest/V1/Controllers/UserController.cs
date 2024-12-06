using Application.Command.UserCommands;
using Application.Command.ViewModels;
using Application.Query.UserQueries;
using Application.Query.ViewModels;
using Hangfire;
using Infrastructure.Services;
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
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public UserController(IMediator mediator, IBackgroundJobClient backgroundJobClient)
        {
            _mediator = mediator;
            _backgroundJobClient = backgroundJobClient;
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

            return result.Success ? Ok(result) : Unauthorized(result.ErrorMessage);
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
                return result ? Ok(true) : BadRequest("User creation failed.");
            }
            catch (Exception)
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
            return result == null ? NotFound("User not found.") : Ok(result);
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        [SwaggerOperation("Change user password")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Invalid old password")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Password changed successfully", typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        public async Task<ActionResult<bool>> ChangePassword([FromBody] ChangePasswordModel request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _mediator.Send(new ChangePasswordCommand(request.UserName, request.OldPassword, request.NewPassword));
                return result ? Ok(true) : Unauthorized("Invalid old password.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("ImportUsers")]
        [Authorize]
        [SwaggerOperation(
           Summary = "Import users from file",
           Description = "Uploads an Excel file to import users",
           OperationId = "ImportUsers",
           Tags = new[] { "User" }
       )]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid file")]
        [SwaggerResponse((int)HttpStatusCode.OK, "User import has been scheduled")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        [Consumes("multipart/form-data")]
        public IActionResult ImportUsers(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file.");
            }

            var filePath = Path.Combine(Path.GetTempPath(), file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            _backgroundJobClient.Enqueue<UserImportService>(service => service.ImportUsersFromExcelAsync(filePath));

            return Ok("User import has been scheduled.");
        }
    }
}
