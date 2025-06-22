using Application.Query.ChatMessageQueries;
using Application.Query.ViewModels.Application.Query.ViewModels;
using Application.Service.Common;
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

        //[LogRequestResponse]
        [HttpPost("AddChatMessage")]
        [SwaggerOperation("Add a new chat message")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Chat message added successfully", typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        public async Task<ActionResult<string>> AddChatMessage([FromBody] AddChatMessageModel request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Pass all required parameters to AddChatMessageCommand
            var result = await _mediator.Send(
                new AddChatMessageCommand(
                    request.ChatSessionId == Guid.Empty ? null : request.ChatSessionId,
                    request.Question,
                    request.SessionName,
                    request.Description
                )
            );

            if (!result.IsSuccess)
            {
                // Return 400 for known failures, 500 for unexpected errors
                if (result.Message?.Contains("Unauthorized", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) == true ||
                    result.Message?.Contains("Invalid", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return BadRequest(result.Message);
                }
                return StatusCode(500, result.Message);
            }

            return Ok(result.Data);
        }

        [HttpGet("GetChatMessagesBySessionId/{sessionId}")]
        [SwaggerOperation("Get chat messages by session ID")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Chat messages retrieved successfully", typeof(PaginatedResult<GetChatMessagesBySessionIdViewModel>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        public async Task<ActionResult<PaginatedResult<GetChatMessagesBySessionIdViewModel>>> GetChatMessagesBySessionId(Guid sessionId, int pageNumber = 1, int pageSize = 10)
        {
            if (sessionId == Guid.Empty) return BadRequest("Invalid session ID");

            var result = await _mediator.Send(new GetBySessionIdQuery(sessionId, pageNumber, pageSize));

            return Ok(result);
        }
    }
}
