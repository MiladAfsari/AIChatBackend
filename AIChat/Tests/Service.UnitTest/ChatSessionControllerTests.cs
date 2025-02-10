using Application.Command.ChatSessionCommands;
using Application.Query.ChatSessionQueries;
using Application.Query.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Rest.V1.Controllers;
using Service.Rest.V1.RequestModels;

namespace Service.UnitTest
{
    public class ChatSessionControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ChatSessionController _controller;

        public ChatSessionControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new ChatSessionController(_mediatorMock.Object);
        }

        [Fact]
        public async Task AddChatSession_ReturnsOkResult_WhenChatSessionIsCreated()
        {
            // Arrange
            var request = new AddChatSessionModel { SessionName = "Test Session", Description = "Test Description" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddChatSessionCommand>(), default)).ReturnsAsync(Guid.NewGuid());

            // Act
            var result = await _controller.AddChatSession(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsType<Guid>(okResult.Value);
        }

        [Fact]
        public async Task AddChatSession_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var request = new AddChatSessionModel { SessionName = "", Description = "Test Description" };
            _controller.ModelState.AddModelError("SessionName", "Required");

            // Act
            var result = await _controller.AddChatSession(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetChatSessions_ReturnsOkResult_WithListOfChatSessions()
        {
            // Arrange
            var chatSessions = new List<GetChatSessionsByUserIdViewModel>
                {
                    new GetChatSessionsByUserIdViewModel { Id = Guid.NewGuid(), SessionName = "Session 1", Description = "Description 1" },
                    new GetChatSessionsByUserIdViewModel { Id = Guid.NewGuid(), SessionName = "Session 2", Description = "Description 2" }
                };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllChatSessionsQueryByUserId>(), default)).ReturnsAsync(chatSessions);

            // Act
            var result = await _controller.GetChatSessions();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<GetChatSessionsByUserIdViewModel>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
    }
}
