using Application.Command.FeedbackCommands;
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
    [Route("api/Feedback")]
    [Authorize]
    [LogException]
    public class FeedbackController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FeedbackController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [LogRequestResponse]
        [HttpPost("AddFeedback")]
        [SwaggerOperation("Add feedback for a chat message")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid request")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Feedback added successfully", typeof(bool))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, "Internal server error")]
        public async Task<ActionResult<bool>> AddFeedback([FromBody] AddFeedbackModel request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _mediator.Send(new AddFeedbackCommand(request.ChatMessageId, request.Rating));

            return result ? Ok(result) : StatusCode(500, "Error adding feedback");
        }
    }
}
