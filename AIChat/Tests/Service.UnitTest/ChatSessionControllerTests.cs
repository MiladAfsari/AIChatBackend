using Application.Command.ChatSessionCommands;
using Application.Query.ChatSessionQueries;
using Application.Query.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Rest.V1.Controllers;
using Service.Rest.V1.RequestModels;
using System.Net;

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
            var request = new AddChatSessionModel
            {
                SessionName = "Test Session",
                Description = "Test Description",
                ApplicationUserId = Guid.NewGuid()
            };
            var expectedGuid = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddChatSessionCommand>(), default)).ReturnsAsync(expectedGuid);

            // Act
            var result = await _controller.AddChatSession(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.Equal(expectedGuid, okResult.Value);
        }

        [Fact]
        public async Task AddChatSession_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("SessionName", "Required");

            // Act
            var result = await _controller.AddChatSession(new AddChatSessionModel());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task GetChatSessionsByUserId_ReturnsOkResult_WithChatSessions()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var chatSessions = new List<GetChatSessionsByUserIdViewModel>
                {
                    new GetChatSessionsByUserIdViewModel { Id = Guid.NewGuid(), SessionName = "Session 1", Description = "Description 1" },
                    new GetChatSessionsByUserIdViewModel { Id = Guid.NewGuid(), SessionName = "Session 2", Description = "Description 2" }
                };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetByUserIdQuery>(), default)).ReturnsAsync(chatSessions);

            // Act
            var result = await _controller.GetChatSessionsByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.Equal(chatSessions, okResult.Value);
        }

        [Fact]
        public async Task GetChatSessionsByUserId_ReturnsBadRequest_WhenUserIdIsInvalid()
        {
            // Act
            var result = await _controller.GetChatSessionsByUserId(Guid.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }
    }
}
