using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.API.Dtos.WorkShift;
using UserManagement.API.Repositories.Interfaces;
using UserManagement.API.Services.Behaviours;

namespace UserManagementTest.UnitTests.Services
{
    public class WorkShiftsServiceTests
    {
        private readonly Mock<ILogger<WorkShiftsService>> _loggerMock;
        private readonly Mock<IWorkShiftsRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly WorkShiftsService _service;

        public WorkShiftsServiceTests()
        {
            _loggerMock = new Mock<ILogger<WorkShiftsService>>();
            _repositoryMock = new Mock<IWorkShiftsRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new WorkShiftsService(_loggerMock.Object, _repositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task UpdateShift_ShouldReturnFalse_WhenShiftNotFound()
        {
            var updateDto = new UpdateWorkShiftDto
            {
                Id = Guid.NewGuid(),
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(12, 0, 0),
                GracePeriod = 10
            };

            _repositoryMock.Setup(r => r.GetShiftById(updateDto.Id))
                           .ReturnsAsync((WorkShift)null);

            var result = await _service.UpdateShift(updateDto);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateShift_ShouldReturnFalse_WhenGracePeriodInvalid()
        {
            var updateDto = new UpdateWorkShiftDto
            {
                Id = Guid.NewGuid(),
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(9, 0, 0),
                GracePeriod = -5
            };

            var result = await _service.UpdateShift(updateDto);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateShift_ShouldReturnTrue_WhenValid()
        {
            var updateDto = new UpdateWorkShiftDto
            {
                Id = Guid.NewGuid(),
                StartTime = new TimeSpan(8, 0, 0),
                EndTime = new TimeSpan(12, 0, 0),
                GracePeriod = 5
            };

            var workShift = new WorkShift { Id = updateDto.Id };

            _repositoryMock.Setup(r => r.GetShiftById(updateDto.Id))
                           .ReturnsAsync(workShift);

            _repositoryMock.Setup(r => r.Save())
                           .ReturnsAsync(true);

            var result = await _service.UpdateShift(updateDto);

            Assert.True(result);
        }

        [Fact]
        public async Task GetShifts_ShouldReturnMappedResult()
        {
            var shifts = new List<WorkShift>
        {
            new WorkShift { Id = Guid.NewGuid(), StartTime = new TimeSpan(8,0,0), EndTime = new TimeSpan(12,0,0) }
        };

            var expectedDtos = new List<WorkShiftResponseDto>
        {
            new WorkShiftResponseDto { Id = shifts[0].Id, StartTime = shifts[0].StartTime, EndTime = shifts[0].EndTime }
        };

            _repositoryMock.Setup(r => r.GetShifts())
                           .ReturnsAsync(shifts);

            _mapperMock.Setup(m => m.Map<List<WorkShift>, List<WorkShiftResponseDto>>(shifts))
                       .Returns(expectedDtos);

            var result = await _service.GetShifts();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(expectedDtos[0].Id, result[0].Id);
        }
    }

}
