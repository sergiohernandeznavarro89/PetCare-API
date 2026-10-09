using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PetCare.API.Controllers;
using PetCare.API.DTOs;
using PetCare.API.Features.Auth;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PetCare.API.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AuthController(_mediatorMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsOk_WithAuthResponseDto()
        {
            // Arrange
            var createDto = new RegisterDto { Email = "juan@test.com", Password = "PassWord123" };
            var expectedResponse = new AuthResponseDto { Token = "register-token" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterUserCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Register(createDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUser = okResult.Value.Should().BeAssignableTo<AuthResponseDto>().Subject;
            returnedUser.Token.Should().Be("register-token");
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOk_WithToken()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "juan@test.com", Password = "PassWord123" };
            var expectedResponse = new AuthResponseDto { Token = "jwt-token-string" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var authResponse = okResult.Value.Should().BeAssignableTo<AuthResponseDto>().Subject;
            authResponse.Token.Should().Be("jwt-token-string");
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "juan@test.com", Password = "wrongpassword" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<LoginUserCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new System.Exception("Invalid credentials"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}


