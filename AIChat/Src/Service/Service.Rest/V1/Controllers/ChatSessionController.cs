using Application.Command.ChatSessionCommands;
using Application.Query.ChatSessionQueries;
using Application.Query.ViewModels;
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
    [Route("api/ChatSession")]
    [Authorize]
    [LogException]
    public class ChatSessionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatSessionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [LogRequestResponse]
        [HttpPost("AddChatSession")]
        [SwaggerOperation("Add a new chat session")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Chat session created successfully", typeof(Guid))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        public async Task<ActionResult<Guid>> AddChatSession([FromBody] AddChatSessionModel request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _mediator.Send(new AddChatSessionCommand(request.SessionName, request.Description));

            return result == Guid.Empty ? StatusCode(500, "Error adding chat session") : Ok(result);
        }

        [HttpGet("GetChatSessions")]
        [SwaggerOperation("Get all chat sessions")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Chat sessions retrieved successfully", typeof(IEnumerable<GetChatSessionsByUserIdViewModel>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        public async Task<ActionResult<IEnumerable<GetChatSessionsByUserIdViewModel>>> GetChatSessions()
        {
            var result = await _mediator.Send(new GetAllChatSessionsQueryByUserId());

            return Ok(result);
        }
        [HttpDelete("DeleteChatSessionWithMessages/{chatSessionId}")]
        [SwaggerOperation("Delete a chat session along with its messages")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Chat session and its messages deleted successfully")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Chat session not found")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Unauthorized access to chat session")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        public async Task<ActionResult> DeleteChatSessionWithMessages(Guid chatSessionId)
        {
            var result = await _mediator.Send(new DeleteChatSessionWithMessagesCommand(chatSessionId));

            if (!result.IsSuccess)
            {
                if (result.Message == "Chat session not found.")
                    return NotFound(result.Message);
                if (result.Message == "Unauthorized access to chat session.")
                    return Unauthorized(result.Message);
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPatch("UpdateChatSessionName")]
        [SwaggerOperation("Update the name of a chat session")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Chat session name updated successfully")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "Chat session not found")]
        [SwaggerResponse((int)HttpStatusCode.Unauthorized, "Unauthorized access to chat session")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        public async Task<ActionResult> UpdateChatSessionName([FromBody] UpdateChatSessionNameModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (model == null || string.IsNullOrWhiteSpace(model.NewSessionName))
                return BadRequest("New session name must not be empty.");

            var result = await _mediator.Send(new UpdateChatSessionNameBySessionIdCommand(model.chatSessionId, model.NewSessionName));
            if (!result.IsSuccess)
            {
                if (result.Message == "Chat session not found.")
                    return NotFound(result.Message);
                if (result.Message == "Unauthorized access to chat session.")
                    return Unauthorized(result.Message);
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }
    }
}
