using Application.Command.ChatMessageCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Rest.V1.Controllers;
using Service.Rest.V1.RequestModels;
using System.Net;

namespace Service.UnitTest
{
    public class ChatMessageControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ChatMessageController _controller;

        public ChatMessageControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ChatMessageController(_mediatorMock.Object);
        }

        [Fact]
        public async Task AddChatMessage_ReturnsOkResult_WhenMessageIsAddedSuccessfully()
        {
            // Arrange
            var addChatMessageModel = new AddChatMessageModel
            {
                ChatSessionId = Guid.NewGuid(),
                Question = "What is the time?",
                Answer = "It's 2 PM."
            };
            var expectedGuid = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddChatMessageCommand>(), default)).ReturnsAsync((Guid?)expectedGuid);

            // Act
            var result = await _controller.AddChatMessage(addChatMessageModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        [Fact]
        public async Task AddChatMessage_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var addChatMessageModel = new AddChatMessageModel();
            _controller.ModelState.AddModelError("ChatSessionId", "Required");

            // Act
            var result = await _controller.AddChatMessage(addChatMessageModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var modelState = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(modelState.ContainsKey("ChatSessionId"));
            Assert.Equal("Required", ((string[])modelState["ChatSessionId"])[0]);
        }

        [Fact]
        public async Task AddChatMessage_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var addChatMessageModel = new AddChatMessageModel
            {
                ChatSessionId = Guid.NewGuid(),
                Question = "What is the time?",
                Answer = "It's 2 PM."
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddChatMessageCommand>(), default)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.AddChatMessage(addChatMessageModel);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.InternalServerError, internalServerErrorResult.StatusCode);
            Assert.Equal("Internal server error", internalServerErrorResult.Value);
        }
    }
}
