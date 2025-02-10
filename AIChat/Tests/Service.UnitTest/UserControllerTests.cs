using Application.Command.UserCommands;
using Application.Command.ViewModels;
using Application.Query.UserQueries;
using Application.Query.ViewModels;
using Hangfire;
using MediatR;
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
        private readonly AuthenticateController _controller;

        public UserControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _backgroundJobClientMock = new Mock<IBackgroundJobClient>();
            _controller = new AuthenticateController(_mediatorMock.Object, _backgroundJobClientMock.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnOkResult_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginModel = new LoginModel { UserName = "testuser", Password = "password" };
            var loginViewModel = new LoginViewModel { Success = true, Token = "token" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), default)).ReturnsAsync(loginViewModel);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<LoginViewModel>(okResult.Value);
            Assert.True(returnValue.Success);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorizedResult_WhenLoginFails()
        {
            // Arrange
            var loginModel = new LoginModel { UserName = "testuser", Password = "wrongpassword" };
            var loginViewModel = new LoginViewModel { Success = false, ErrorMessage = "Invalid username or password" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<LoginCommand>(), default)).ReturnsAsync(loginViewModel);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid username or password", unauthorizedResult.Value);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnOkResult_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var createUserModel = new CreateUserModel { UserName = "newuser", Password = "password", Role = "User", FullName = "New User", DepartmentId = 1 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), default)).ReturnsAsync(true);

            // Act
            var result = await _controller.CreateUser(createUserModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenUserCreationFails()
        {
            // Arrange
            var createUserModel = new CreateUserModel { UserName = "newuser", Password = "password", Role = "User", FullName = "New User", DepartmentId = 1 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateUserCommand>(), default)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateUser(createUserModel);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("User creation failed.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetUserByToken_ShouldReturnOkResult_WhenUserIsFound()
        {
            // Arrange
            var userViewModel = new UserViewModel { Id = Guid.NewGuid(), UserName = "testuser", FullName = "Test User", Email = "testuser@example.com", DepartmentId = 1 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByTokenQuery>(), default)).ReturnsAsync(userViewModel);

            // Act
            var result = await _controller.GetUserByToken();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<UserViewModel>(okResult.Value);
            Assert.Equal("testuser", returnValue.UserName);
        }

        [Fact]
        public async Task GetUserByToken_ShouldReturnNotFoundResult_WhenUserIsNotFound()
        {
            // Arrange
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByTokenQuery>(), default)).ReturnsAsync((UserViewModel)null);

            // Act
            var result = await _controller.GetUserByToken();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("User not found.", notFoundResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnOkResult_WhenPasswordIsChangedSuccessfully()
        {
            // Arrange
            var changePasswordModel = new ChangePasswordModel { OldPassword = "oldpassword", NewPassword = "newpassword" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ChangePasswordCommand>(), default)).ReturnsAsync(true);

            // Act
            var result = await _controller.ChangePassword(changePasswordModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task ChangePassword_ShouldReturnUnauthorizedResult_WhenOldPasswordIsInvalid()
        {
            // Arrange
            var changePasswordModel = new ChangePasswordModel { OldPassword = "wrongoldpassword", NewPassword = "newpassword" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<ChangePasswordCommand>(), default)).ReturnsAsync(false);

            // Act
            var result = await _controller.ChangePassword(changePasswordModel);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Equal("Invalid old password.", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Logout_ShouldReturnOkResult_WhenLogoutIsSuccessful()
        {
            // Arrange
            var logOutViewModel = new LogOutViewModel { Success = true };
            _mediatorMock.Setup(m => m.Send(It.IsAny<LogOutCommand>(), default)).ReturnsAsync(logOutViewModel);

            // Act
            var result = await _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<LogOutViewModel>(okResult.Value);
            Assert.True(returnValue.Success);
        }

        [Fact]
        public async Task Logout_ShouldReturnBadRequestResult_WhenLogoutFails()
        {
            // Arrange
            var logOutViewModel = new LogOutViewModel { Success = false, ErrorMessage = "Logout failed" };
            _mediatorMock.Setup(m => m.Send(It.IsAny<LogOutCommand>(), default)).ReturnsAsync(logOutViewModel);

            // Act
            var result = await _controller.Logout();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Logout failed", badRequestResult.Value);
        }
    }
}
