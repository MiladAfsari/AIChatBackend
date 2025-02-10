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

        [Fact]
        public async Task AddFeedback_ReturnsOkResult_WhenFeedbackIsAddedSuccessfully()
        {
            // Arrange
            var request = new AddFeedbackModel { ChatMessageId = Guid.NewGuid(), Rating = 5 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddFeedbackCommand>(), default)).ReturnsAsync(true);

            // Act
            var result = await _controller.AddFeedback(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task AddFeedback_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var request = new AddFeedbackModel { ChatMessageId = Guid.NewGuid(), Rating = 5 };
            _controller.ModelState.AddModelError("ChatMessageId", "Required");

            // Act
            var result = await _controller.AddFeedback(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task AddFeedback_ReturnsInternalServerError_WhenFeedbackIsNotAdded()
        {
            // Arrange
            var request = new AddFeedbackModel { ChatMessageId = Guid.NewGuid(), Rating = 5 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddFeedbackCommand>(), default)).ReturnsAsync(false);

            // Act
            var result = await _controller.AddFeedback(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
            Assert.Equal("Error adding feedback", statusCodeResult.Value);
        }
    }
}
