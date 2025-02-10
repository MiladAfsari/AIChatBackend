using Application.Command.ChatMessageCommands;
using Application.Query.ChatMessageQueries;
using Application.Query.ViewModels.Application.Query.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Rest.V1.Controllers;
using Service.Rest.V1.RequestModels;

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
        public async Task AddChatMessage_ReturnsOkResult_WhenMessageIsAdded()
        {
            // Arrange
            var request = new AddChatMessageModel
            {
                ChatSessionId = Guid.NewGuid(),
                Question = "Test question",
                Answer = "Test answer"
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddChatMessageCommand>(), default))
                         .ReturnsAsync(Guid.NewGuid());

            // Act
            var result = await _controller.AddChatMessage(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
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
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetChatMessagesBySessionId_ReturnsOkResult_WithMessages()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var messages = new List<GetChatMessagesBySessionIdViewModel>
                {
                    new GetChatMessagesBySessionIdViewModel
                    {
                        ChatSessionId = sessionId,
                        ChatMessageId = Guid.NewGuid(),
                        Question = "Test question",
                        Answer = "Test answer"
                    }
                };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetBySessionIdQuery>(), default))
                         .ReturnsAsync(messages);

            // Act
            var result = await _controller.GetChatMessagesBySessionId(sessionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(messages, okResult.Value);
        }

        [Fact]
        public async Task GetChatMessagesBySessionId_ReturnsBadRequest_WhenSessionIdIsEmpty()
        {
            // Act
            var result = await _controller.GetChatMessagesBySessionId(Guid.Empty);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}
