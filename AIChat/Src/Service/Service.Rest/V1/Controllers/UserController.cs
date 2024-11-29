using Application.Command.UserCommands;
using Application.Query.UserQueries;
using Application.Query.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Rest.V1.RequestModels;

namespace Service.Rest.V1.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> CreateUser([FromBody] CreateUserModel request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _mediator.Send(new CreateUserCommand(request.UserName, request.Password, request.Role));

            if (result)
                return Ok(true);

            return BadRequest(false);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<UserViewModel>> GetUserById([FromRoute]string id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));

            if (result == null)
            {
                return NotFound("User not found.");
            }

            return Ok(result);
        }
    }
}
