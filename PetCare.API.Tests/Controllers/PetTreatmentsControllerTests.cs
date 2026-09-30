using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PetCare.API.Controllers;
using PetCare.API.DTOs;
using PetCare.API.Features.PetTreatments;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PetCare.API.Tests.Controllers
{
    public class PetTreatmentsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly PetTreatmentsController _controller;
        private readonly Guid _userId;

        public PetTreatmentsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PetTreatmentsController(_mediatorMock.Object);
            
            _userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _userId.ToString()),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task AssignTreatment_ReturnsOk_WithAssignedTreatment()
        {
            // Arrange
            var createDto = new CreatePetTreatmentDto 
            { 
                PetId = Guid.NewGuid(), 
                TreatmentId = Guid.NewGuid(), 
                FrequencyInDays = 30, 
                NextDueDate = DateTime.Now.AddDays(30), 
                Notify = true 
            };
            
            var expectedResponse = new PetTreatmentDto
            {
                Id = Guid.NewGuid(),
                PetId = createDto.PetId,
                TreatmentId = createDto.TreatmentId,
                FrequencyInDays = createDto.FrequencyInDays,
                NextDueDate = createDto.NextDueDate,
                Notify = createDto.Notify,
                IsActive = true
            };

            _mediatorMock.Setup(m => m.Send(It.Is<CreatePetTreatmentCommand>(c => c.UserId == _userId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.AssignTreatment(createDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTreatment = okResult.Value.Should().BeAssignableTo<PetTreatmentDto>().Subject;
            returnedTreatment.Notify.Should().BeTrue();
            returnedTreatment.PetId.Should().Be(createDto.PetId);
        }

        [Fact]
        public async Task AssignTreatment_ThrowsUnauthorizedAccessException_ReturnsForbid()
        {
            // Arrange
            var createDto = new CreatePetTreatmentDto { PetId = Guid.NewGuid(), TreatmentId = Guid.NewGuid() };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePetTreatmentCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new UnauthorizedAccessException("You do not own this pet."));

            // Act
            var result = await _controller.AssignTreatment(createDto);

            // Assert
            var forbidResult = result.Result.Should().BeOfType<ForbidResult>().Subject;
        }
    }
}
