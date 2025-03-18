using Application.Query.ChatMessageQueries;
using Application.Query.ViewModels.Application.Query.ViewModels;
using Application.Service.Common;
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
        public async Task AddChatMessage_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("ChatSessionId", "Required");

            // Act
            var result = await _controller.AddChatMessage(new AddChatMessageModel());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task GetChatMessagesBySessionId_ReturnsOkResult_WhenSessionIdIsValid()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var queryResult = new PaginatedResult<GetChatMessagesBySessionIdViewModel>
            {
                Items = new List<GetChatMessagesBySessionIdViewModel>(),
                PageNumber = 1,
                PageSize = 10,
                TotalCount = 0
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetBySessionIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(queryResult);

            // Act
            var result = await _controller.GetChatMessagesBySessionId(sessionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
            Assert.Equal(queryResult, okResult.Value);
        }

        [Fact]
        public async Task GetChatMessagesBySessionId_ReturnsBadRequest_WhenSessionIdIsInvalid()
        {
            // Act
            var result = await _controller.GetChatMessagesBySessionId(Guid.Empty);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
            Assert.Equal("Invalid session ID", badRequestResult.Value);
        }
    }
}
