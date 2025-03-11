using Application.Query.ChatMessageQueries;
using Application.Query.ViewModels.Application.Query.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Rest.Attributes.LogException;
using Service.Rest.Attributes.LogRequestResponse;
using Service.Rest.V1.RequestModels;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace Service.Rest.V1.Controllers
{
    [ApiController]
    [Route("api/ChatMessage")]
    [Authorize]
    [LogException]
    public class ChatMessageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatMessageController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [LogRequestResponse]
        [HttpPost("AddChatMessage")]
        [SwaggerOperation("Add a new chat message")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Chat message added successfully", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        public async Task<ActionResult<string>> AddChatMessage([FromBody] AddChatMessageModel request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _mediator.Send(new AddChatMessageCommand(request.ChatSessionId, request.Question));

            if (!result.IsSuccess)
            {
                return StatusCode(500, result.Message);
            }

            return Ok(result.Data);
        }

        [HttpGet("GetChatMessagesBySessionId/{sessionId}")]
        [SwaggerOperation("Get chat messages by session ID")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Chat messages retrieved successfully", typeof(IEnumerable<GetChatMessagesBySessionIdViewModel>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        public async Task<ActionResult<IEnumerable<GetChatMessagesBySessionIdViewModel>>> GetChatMessagesBySessionId(Guid sessionId)
        {
            if (sessionId == Guid.Empty) return BadRequest("Invalid session ID");

            var result = await _mediator.Send(new GetBySessionIdQuery(sessionId));

            return Ok(result);
        }
    }
}
