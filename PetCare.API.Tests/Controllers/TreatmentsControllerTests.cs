using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PetCare.API.Controllers;
using PetCare.API.DTOs;
using PetCare.API.Features.Treatments;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PetCare.API.Tests.Controllers
{
    public class TreatmentsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly TreatmentsController _controller;

        public TreatmentsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new TreatmentsController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetTreatments_ReturnsOk_WithListOfTreatments()
        {
            // Arrange
            var expectedTreatments = new List<TreatmentDto>
            {
                new TreatmentDto { Id = System.Guid.NewGuid(), Name = "Vacuna Rabia", Description = "Anual", Type = "Vacuna" },
                new TreatmentDto { Id = System.Guid.NewGuid(), Name = "Desparasitación", Description = "Trimestral", Type = "Pastilla" }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetTreatmentsQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedTreatments);

            // Act
            var result = await _controller.GetTreatments();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTreatments = okResult.Value.Should().BeAssignableTo<IEnumerable<TreatmentDto>>().Subject;
            returnedTreatments.Should().HaveCount(2);
        }

        [Fact]
        public async Task PostTreatment_ReturnsOk_WithCreatedTreatment()
        {
            // Arrange
            var createDto = new CreateTreatmentDto { Name = "Chip", Description = "Identificación", Type = "Otro" };
            var expectedTreatment = new TreatmentDto { Id = System.Guid.NewGuid(), Name = createDto.Name, Description = createDto.Description, Type = createDto.Type };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateTreatmentCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(expectedTreatment);

            // Act
            var result = await _controller.PostTreatment(createDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTreatment = okResult.Value.Should().BeAssignableTo<TreatmentDto>().Subject;
            returnedTreatment.Name.Should().Be("Chip");
        }

        [Fact]
        public async Task PutTreatment_WhenSuccess_ReturnsNoContent()
        {
            // Arrange
            var id = System.Guid.NewGuid();
            var updateDto = new CreateTreatmentDto { Name = "Vacuna Rabia", Description = "Cada año", Type = "Vacuna" };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateTreatmentCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            // Act
            var result = await _controller.PutTreatment(id, updateDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteTreatment_WhenSuccess_ReturnsNoContent()
        {
            // Arrange
            var id = System.Guid.NewGuid();

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteTreatmentCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTreatment(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
