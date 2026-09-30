using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PetCare.API.Controllers;
using PetCare.API.DTOs;
using PetCare.API.Features.Pets;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace PetCare.API.Tests.Controllers
{
    public class PetsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly PetsController _controller;
        private readonly Guid _userId;

        public PetsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new PetsController(_mediatorMock.Object);
            
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
        public async Task GetPets_ReturnsOk_WithListOfPets()
        {
            // Arrange
            var expectedPets = new List<PetDto>
            {
                new PetDto { Id = Guid.NewGuid(), Name = "Firulais", Species = "Dog", Breed = "Labrador", DateOfBirth = DateTime.Now.AddYears(-3) },
                new PetDto { Id = Guid.NewGuid(), Name = "Mishi", Species = "Cat", Breed = "Siamese", DateOfBirth = DateTime.Now.AddYears(-2) }
            };

            _mediatorMock.Setup(m => m.Send(It.Is<GetPetsQuery>(q => q.UserId == _userId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedPets);

            // Act
            var result = await _controller.GetPets();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedPets = okResult.Value.Should().BeAssignableTo<IEnumerable<PetDto>>().Subject;
            returnedPets.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetPet_WhenPetExists_ReturnsOk()
        {
            // Arrange
            var petId = Guid.NewGuid();
            var expectedPet = new PetDto { Id = petId, Name = "Firulais", Species = "Dog", Breed = "Labrador", DateOfBirth = DateTime.Now.AddYears(-3) };

            _mediatorMock.Setup(m => m.Send(It.Is<GetPetByIdQuery>(q => q.Id == petId && q.UserId == _userId), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedPet);

            // Act
            var result = await _controller.GetPet(petId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedPet = okResult.Value.Should().BeAssignableTo<PetDto>().Subject;
            returnedPet.Id.Should().Be(petId);
        }

        [Fact]
        public async Task GetPet_WhenPetDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var petId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetPetByIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((PetDto?)null);

            // Act
            var result = await _controller.GetPet(petId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task PostPet_ReturnsCreated()
        {
            // Arrange
            var createDto = new CreatePetDto { Name = "NewPet", Species = "Bird", Breed = "Parrot", DateOfBirth = DateTime.Now.AddYears(-1) };
            var expectedPet = new PetDto { Id = Guid.NewGuid(), Name = createDto.Name, Species = createDto.Species, Breed = createDto.Breed, DateOfBirth = createDto.DateOfBirth };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreatePetCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedPet);

            // Act
            var result = await _controller.PostPet(createDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(PetsController.GetPet));
            var returnedPet = createdResult.Value.Should().BeAssignableTo<PetDto>().Subject;
            returnedPet.Name.Should().Be("NewPet");
        }
    }
}

