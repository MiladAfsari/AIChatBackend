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
            var request = new AddFeedbackModel
            {
                ChatMessageId = Guid.NewGuid(),
                ApplicationUserId = "user123",
                IsLiked = true
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddFeedbackCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

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
            var request = new AddFeedbackModel();
            _controller.ModelState.AddModelError("ChatMessageId", "Required");

            // Act
            var result = await _controller.AddFeedback(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task AddFeedback_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new AddFeedbackModel
            {
                ChatMessageId = Guid.NewGuid(),
                ApplicationUserId = "user123",
                IsLiked = true
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddFeedbackCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception());

            // Act
            var result = await _controller.AddFeedback(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);
        }
    }
}
