using Application.Command.ChatSessionCommands;
using Application.Query.ChatSessionQueries;
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
    [Route("api/ChatSession")]
    [Authorize]
    public class ChatSessionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatSessionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("AddChatSession")]
        [SwaggerOperation("Add a new chat session")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Chat session created successfully", typeof(Guid))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        public async Task<ActionResult<Guid>> AddChatSession([FromBody] AddChatSessionModel request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _mediator.Send(new AddChatSessionCommand(request.SessionName, request.Description, request.ApplicationUserId));

            return result == Guid.Empty ? StatusCode(500, "Error adding chat session") : Ok(result);
        }

        [HttpGet("GetChatSessionsByUserId/{userId}")]
        [SwaggerOperation("Get chat sessions by user ID")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Chat sessions retrieved successfully", typeof(IEnumerable<GetChatSessionsByUserIdViewModel>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        public async Task<ActionResult<IEnumerable<GetChatSessionsByUserIdViewModel>>> GetChatSessionsByUserId(Guid userId)
        {
            if (userId == Guid.Empty) return BadRequest("Invalid user ID");

            var result = await _mediator.Send(new GetByUserIdQuery(userId));

            return Ok(result);
        }
    }
}
