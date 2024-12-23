using Application.Command.UserCommands;
using Application.Command.ViewModels;
using Application.Query.UserQueries;
using Application.Query.ViewModels;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Service.Rest.V1.Controllers;
using Service.Rest.V1.RequestModels;

namespace Service.UnitTest
{
    public class UserControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IBackgroundJobClient> _backgroundJobClientMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _backgroundJobClientMock = new Mock<IBackgroundJobClient>();
            _controller = new UserController(_mediatorMock.Object, _backgroundJobClientMock.Object);
        }

        [Fact]
        public async Task Login_ReturnsOkResult_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginModel = new LoginModel { UserName = "testuser", Password = "password" };
            var loginResult = new LoginViewModel { Success = true, Token = "token" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), default)).ReturnsAsync(loginResult);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(loginResult, okResult.Value);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorizedResult_WhenLoginFails()
        {
            // Arrange
            var loginModel = new LoginModel { UserName = "testuser", Password = "password" };
            var loginResult = new LoginViewModel { Success = false, ErrorMessage = "Invalid username or password" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), default)).ReturnsAsync(loginResult);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal(loginResult.ErrorMessage, unauthorizedResult.Value);
        }

        [Fact]
        public async Task CreateUser_ReturnsOkResult_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var createUserModel = new CreateUserModel { UserName = "testuser", Password = "password", Role = "User", FullName = "Test User" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), default)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateUser(createUserModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task CreateUser_ReturnsBadRequest_WhenUserCreationFails()
        {
            // Arrange
            var createUserModel = new CreateUserModel { UserName = "testuser", Password = "password", Role = "User", FullName = "Test User" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), default)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateUser(createUserModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("User creation failed.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetUserByUserName_ReturnsOkResult_WhenUserIsFound()
        {
            // Arrange
            var username = "testuser";
            var userViewModel = new UserViewModel { UserName = username, FullName = "Test User" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByUserNameQuery>(), default)).ReturnsAsync(userViewModel);

            // Act
            var result = await _controller.GetUserByUserName(username);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(userViewModel, okResult.Value);
        }

        [Fact]
        public async Task GetUserByUserName_ReturnsNotFoundResult_WhenUserIsNotFound()
        {
            // Arrange
            var username = "testuser";
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByUserNameQuery>(), default)).ReturnsAsync((UserViewModel)null);

            // Act
            var result = await _controller.GetUserByUserName(username);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ReturnsOkResult_WhenPasswordIsChangedSuccessfully()
        {
            // Arrange
            var changePasswordModel = new ChangePasswordModel { UserName = "testuser", OldPassword = "oldpassword", NewPassword = "newpassword" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ChangePasswordCommand>(), default)).ReturnsAsync(true);

            // Act
            var result = await _controller.ChangePassword(changePasswordModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ReturnsUnauthorizedResult_WhenOldPasswordIsInvalid()
        {
            // Arrange
            var changePasswordModel = new ChangePasswordModel { UserName = "testuser", OldPassword = "oldpassword", NewPassword = "newpassword" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ChangePasswordCommand>(), default)).ReturnsAsync(false);

            // Act
            var result = await _controller.ChangePassword(changePasswordModel);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid old password.", unauthorizedResult.Value);
        }

        [Fact]
        public void ImportUsers_ReturnsOkResult_WhenFileIsValid()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            var fileName = "test.xlsx";
            var filePath = Path.Combine(Path.GetTempPath(), fileName);
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(1);
            fileMock.Setup(f => f.CopyTo(It.IsAny<Stream>())).Callback<Stream>(s => s.WriteByte(0));

            // Act
            var result = _controller.ImportUsers(fileMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User import has been scheduled.", okResult.Value);
        }

        [Fact]
        public void ImportUsers_ReturnsBadRequest_WhenFileIsInvalid()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            // Act
            var result = _controller.ImportUsers(fileMock.Object);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid file.", badRequestResult.Value);
        }

        [Fact]
        public async Task Logout_ReturnsOkResult_WhenLogoutIsSuccessful()
        {
            // Arrange
            var userName = "testuser";
            var logoutResult = new LogOutViewModel { Success = true };
            _mediatorMock.Setup(m => m.Send(It.IsAny<LogOutCommand>(), default)).ReturnsAsync(logoutResult);

            // Act
            var result = await _controller.Logout(userName);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(logoutResult, okResult.Value);
        }

        [Fact]
        public async Task Logout_ReturnsBadRequest_WhenLogoutFails()
        {
            // Arrange
            var userName = "testuser";
            var logoutResult = new LogOutViewModel { Success = false, ErrorMessage = "Logout failed" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<LogOutCommand>(), default)).ReturnsAsync(logoutResult);

            // Act
            var result = await _controller.Logout(userName);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(logoutResult.ErrorMessage, badRequestResult.Value);
        }
    }
}
