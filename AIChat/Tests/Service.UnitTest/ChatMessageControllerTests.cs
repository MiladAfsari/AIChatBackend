using Application.Command.ChatMessageCommands;
using Application.Query.ChatMessageQueries;
using Application.Query.ViewModels.Application.Query.ViewModels;
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
        public async Task AddChatMessage_ReturnsOkResult_WhenRequestIsValid()
        {
            // Arrange
            var request = new AddChatMessageModel
            {
                ChatSessionId = Guid.NewGuid(),
                Question = "What is your name?",
                Answer = "GitHub Copilot"
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddChatMessageCommand>(), default))
                         .ReturnsAsync(Guid.NewGuid());

            // Act
            var result = await _controller.AddChatMessage(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task AddChatMessage_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var request = new AddChatMessageModel();
            _controller.ModelState.AddModelError("ChatSessionId", "Required");

            // Act
            var result = await _controller.AddChatMessage(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task GetChatMessagesBySessionId_ReturnsOkResult_WhenSessionIdIsValid()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var chatMessages = new List<GetChatMessagesBySessionIdViewModel>
                {
                    new GetChatMessagesBySessionIdViewModel
                    {
                        ChatSessionId = sessionId,
                        Question = "What is your name?",
                        Answer = "GitHub Copilot",
                        Feedback = new FeedbackViewModel()
                    }
                };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetBySessionIdQuery>(), default))
                         .ReturnsAsync(chatMessages);

            // Act
            var result = await _controller.GetChatMessagesBySessionId(sessionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        }

        [Fact]
        public async Task GetChatMessagesBySessionId_ReturnsBadRequest_WhenSessionIdIsInvalid()
        {
            // Act
            var result = await _controller.GetChatMessagesBySessionId(Guid.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }
    }
}
