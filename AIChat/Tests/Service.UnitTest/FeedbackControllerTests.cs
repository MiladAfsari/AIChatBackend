using Application.Command.FeedbackCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Rest.V1.Controllers;
using Service.Rest.V1.RequestModels;
using System.Net;

namespace Service.UnitTest
{
    public class FeedbackControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly FeedbackController _controller;

        public FeedbackControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new FeedbackController(_mediatorMock.Object);
        }

        //[Fact]
        //public async Task AddFeedback_ReturnsOkResult_WhenFeedbackIsAddedSuccessfully()
        //{
        //    // Arrange
        //    var request = new AddFeedbackModel { ChatMessageId = Guid.CreateVersion7(), Rating = 5 };
        //    var commandResult = new CommandResult<string> { IsSuccess = true, Data = "Feedback added successfully" };
        //    _mediatorMock.Setup(m => m.Send(It.IsAny<AddFeedbackCommand>(), default)).ReturnsAsync(commandResult);

        //    // Act
        //    var result = await _controller.AddFeedback(request);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //    Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        //    Assert.Equal("Feedback added successfully", okResult.Value);
        //}

        [Fact]
        public async Task AddFeedback_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("ChatMessageId", "Required");

            // Act
            var result = await _controller.AddFeedback(new AddFeedbackModel());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        //[Fact]
        //public async Task AddFeedback_ReturnsInternalServerError_WhenFeedbackAdditionFails()
        //{
        //    // Arrange
        //    var request = new AddFeedbackModel { ChatMessageId = Guid.CreateVersion7(), Rating = 5 };
        //    var commandResult = new CommandResult<string> { IsSuccess = false, Message = "Internal server error" };
        //    _mediatorMock.Setup(m => m.Send(It.IsAny<AddFeedbackCommand>(), default)).ReturnsAsync(commandResult);

        //    // Act
        //    var result = await _controller.AddFeedback(request);

        //    // Assert
        //    var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        //    Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
        //    Assert.Equal("Internal server error", statusCodeResult.Value);
        //}
    }
}
