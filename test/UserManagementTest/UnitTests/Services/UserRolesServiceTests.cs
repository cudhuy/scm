//using AutoMapper;
//using Domain.Entities;
//using Microsoft.Extensions.Logging;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UserManagement.API.Dtos.Staff;
//using UserManagement.API.Helpers;
//using UserManagement.API.Repositories.Interfaces;
//using UserManagement.API.Services.Behaviours;

//namespace UserManagementTest.UnitTests.Services
//{
//    public class StaffInformationServiceTests
//    {
//        private readonly Mock<IStaffInformationRepository> _staffRepoMock;
//        private readonly Mock<ILogger<StaffInformationService>> _loggerMock;
//        private readonly Mock<IMapper> _mapperMock;
//        private readonly Mock<CloudinaryHelper> _cloudinaryMock;

//        private readonly StaffInformationService _service;

//        public StaffInformationServiceTests()
//        {
//            _staffRepoMock = new Mock<IStaffInformationRepository>();
//            _loggerMock = new Mock<ILogger<StaffInformationService>>();
//            _mapperMock = new Mock<IMapper>();
//            _cloudinaryMock = new Mock<CloudinaryHelper>();

//            _service = new StaffInformationService(
//                _loggerMock.Object,
//                _staffRepoMock.Object,
//                _mapperMock.Object,
//                _cloudinaryMock.Object
//            );
//        }

//        [Fact]
//        public async Task GetAllStaffs_ReturnsPagedStaffList()
//        {
//            // Arrange
//            var staffs = new List<StaffInfo>
//        {
//            new StaffInfo
//            {
//                Tittle = "Leader",
//                SalaryBase = 1000,
//                Type = "FullTime",
//                User = new User
//                {
//                    Id = Guid.NewGuid(),
//                    FullName = "Nguyen Van A",
//                    Email = "a@email.com",
//                    Phone = "123456789",
//                    Dob = DateTime.Parse("1995-01-01"),
//                    Avatar = "avatar.jpg",
//                    Status = "Active",
//                    Role = new UserRole { RoleName = "Admin", Description = "Quản trị viên" }
//                },
//                Branch = new Branch { Id = Guid.NewGuid(), Name = "Chi nhánh 1", Address = "Địa chỉ 1", Type = "Main" }
//            }
//        };

//            _staffRepoMock.Setup(r => r.GetListStaffInfor()).ReturnsAsync(staffs);

//            // Act
//            var result = await _service.GetAllStaffs(1, 10, null, null);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Single(result.Items);
//            Assert.Equal(1, result.TotalItems);
//        }

//        [Fact]
//        public async Task DeleteStaff_ValidId_ReturnsTrue()
//        {
//            // Arrange
//            var id = Guid.NewGuid();
//            _staffRepoMock.Setup(r => r.DeleteStaff(id)).ReturnsAsync(true);

//            // Act
//            var result = await _service.DeleteStaff(id);

//            // Assert
//            Assert.True(result);
//        }

//        [Fact]
//        public async Task DeleteStaff_InvalidId_ReturnsFalse()
//        {
//            // Arrange
//            var id = Guid.NewGuid();
//            _staffRepoMock.Setup(r => r.DeleteStaff(id)).ReturnsAsync(false);

//            // Act
//            var result = await _service.DeleteStaff(id);

//            // Assert
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task CreateStaff_ReturnsTrue()
//        {
//            // Arrange
//            var dto = new CreateStaffDto { BranchId = Guid.NewGuid(), FullName = "Staff A" };
//            _staffRepoMock.Setup(r => r.CreateStaff(dto)).ReturnsAsync(true);

//            // Act
//            var result = await _service.CreateStaff(dto);

//            // Assert
//            Assert.True(result);
//        }

//        [Fact]
//        public async Task UpdateStaff_StaffNotFound_ReturnsFalse()
//        {
//            // Arrange
//            var dto = new UpdateStaffDto { UserId = Guid.NewGuid() };
//            _staffRepoMock.Setup(r => r.GetStaffById(dto.UserId)).ReturnsAsync((StaffInfo?)null);

//            // Act
//            var result = await _service.UpdateStaff(dto);

//            // Assert
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task GetDetailStaffAsync_StaffExists_ReturnsDetail()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            _staffRepoMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(new User
//            {
//                Id = userId,
//                FullName = "Nguyen Van B",
//                Email = "b@email.com",
//                Phone = "123456",
//                Dob = DateTime.Today,
//                Avatar = "img.png",
//                Status = "Active",
//                Role = new UserRole { RoleName = "User" },
//                StaffInfo = new StaffInfo
//                {
//                    Tittle = "Developer",
//                    Type = "FullTime",
//                    SalaryBase = 1000,
//                    Branch = new Branch { Name = "Chi nhánh A" }
//                }
//            });

//            // Act
//            var result = await _service.GetDetailStaffAsync(userId);

//            // Assert
//            Assert.NotNull(result);
//            Assert.Equal("Nguyen Van B", result.FullName);
//        }
//    }
//}
