//using AutoMapper;
//using Domain.Entities;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UserManagement.API.Dtos.Profile;
//using UserManagement.API.Dtos.Staff;
//using UserManagement.API.Helpers;
//using UserManagement.API.Repositories.Interfaces;
//using UserManagement.API.Services.Behaviours;

//namespace UserManagementTest.UnitTests.Services
//{
//    public class StaffInformationServiceTests
//    {
//        private readonly Mock<ILogger<StaffInformationService>> _loggerMock = new();
//        private readonly Mock<IStaffInformationRepository> _repoMock = new();
//        private readonly Mock<IMapper> _mapperMock = new();
//        private readonly Mock<CloudinaryHelper> _cloudinaryHelperMock = new();
//        private readonly StaffInformationService _service;

//        public StaffInformationServiceTests()
//        {
//            _service = new StaffInformationService(
//                _loggerMock.Object,
//                _repoMock.Object,
//                _mapperMock.Object,
//                _cloudinaryHelperMock.Object
//            );
//        }

//        [Fact]
//        public async Task GetAllStaffs_ReturnsCorrectResult()
//        {
//            // Arrange
//            var userId = Guid.NewGuid();
//            var staffList = new List<StaffInfo>
//        {
//            new()
//            {
//                User = new User
//                {
//                    Id = userId,
//                    FullName = "Test",
//                    Email = "a@a.com",
//                    Phone = "123",
//                    Dob = DateTime.Today,
//                    Avatar = "avatar",
//                    Status = null,
//                    Role = new UserRole { RoleName = "Admin", Description = "desc" }
//                },
//                Tittle = "Manager",
//                Type = "FullTime",
//                SalaryBase = 1000,
//                Branch = new Branch { Id = Guid.NewGuid(), Name = "Branch1", Address = "Address1", Type = "Main" },
//                CreatedAt = DateTime.Now,
//                UpdatedAt = DateTime.Now
//            }
//        };

//            _repoMock.Setup(r => r.GetListStaffInfor()).ReturnsAsync(staffList);

//            // Act
//            var result = await _service.GetAllStaffs(1, 10, null, null);

//            // Assert
//            Assert.Single(result.Items);
//            Assert.Equal(1, result.TotalItems);
//        }

//        [Fact]
//        public async Task DeleteStaff_StaffExists_ReturnsTrue()
//        {
//            var id = Guid.NewGuid();
//            _repoMock.Setup(r => r.DeleteStaff(id)).ReturnsAsync(true);

//            var result = await _service.DeleteStaff(id);

//            Assert.True(result);
//        }

//        [Fact]
//        public async Task DeleteStaff_StaffNotFound_ReturnsFalse()
//        {
//            var id = Guid.NewGuid();
//            _repoMock.Setup(r => r.DeleteStaff(id)).ReturnsAsync(false);

//            var result = await _service.DeleteStaff(id);

//            Assert.False(result);
//        }

//        [Fact]
//        public async Task UpdateStaff_ValidData_ReturnsTrue()
//        {
//            var dto = new UpdateStaffDto { UserId = Guid.NewGuid() };

//            _repoMock.Setup(r => r.GetStaffById(dto.UserId)).ReturnsAsync(new StaffInfo());
//            _repoMock.Setup(r => r.UpdateStaff(dto)).ReturnsAsync(true);

//            var result = await _service.UpdateStaff(dto);

//            Assert.True(result);
//        }

//        [Fact]
//        public async Task UpdateStaff_StaffNotFound_ReturnsFalse()
//        {
//            var dto = new UpdateStaffDto { UserId = Guid.NewGuid() };

//            _repoMock.Setup(r => r.GetStaffById(dto.UserId)).ReturnsAsync((StaffInfo?)null);

//            var result = await _service.UpdateStaff(dto);

//            Assert.False(result);
//        }

//        [Fact]
//        public async Task CreateStaff_ReturnsTrue()
//        {
//            var dto = new CreateStaffDto();
//            _repoMock.Setup(r => r.CreateStaff(dto)).ReturnsAsync(true);

//            var result = await _service.CreateStaff(dto);

//            Assert.True(result);
//        }

//        [Fact]
//        public async Task GetDetailStaffAsync_ReturnsStaffDetail()
//        {
//            var userId = Guid.NewGuid();
//            var user = new User
//            {
//                Id = userId,
//                FullName = "Name",
//                Email = "mail@mail.com",
//                Phone = "123",
//                Address = "Addr",
//                Dob = DateTime.Now,
//                Avatar = "avatar",
//                Status = null,
//                Role = new UserRole(),
//                StaffInfo = new StaffInfo
//                {
//                    Tittle = "Tittle",
//                    Type = "Type",
//                    CreatedAt = DateTime.Now,
//                    SalaryBase = 1000,
//                    Branch = new Branch { Name = "Branch" }
//                }
//            };

//            _repoMock.Setup(r => r.GetUserByIdAsync(userId)).ReturnsAsync(user);

//            var result = await _service.GetDetailStaffAsync(userId);

//            Assert.NotNull(result);
//            Assert.Equal(userId, result.UserId);
//        }

//        //[Fact]
//        //public async Task UpdateProfile_WithImage_UploadsAndReturnsUpdatedProfile()
//        //{
//        //    var userId = Guid.NewGuid();
//        //    var dto = new UpdateProfileDto { UserId = userId, Image = new FormFile(null, 0, 0, "file", "test.jpg") };

//        //    _repoMock.Setup(r => r.GetUserById(userId)).ReturnsAsync(new User { Avatar = null });
//        //    _cloudinaryHelperMock.Setup(c => c.UploadImage(It.IsAny<List<IFormFile>>()))
//        //        .ReturnsAsync(new CloudinaryUploadResult
//        //        {
//        //            IsSuccess = true,
//        //            Files = new List<FileResultDto> { new FileResultDto { Url = "image.jpg" } }
//        //        });
//        //    _repoMock.Setup(r => r.UpdateProfile(dto, "image.jpg")).Returns(Task.CompletedTask);
//        //    _repoMock.Setup(r => r.GetUserById(userId)).ReturnsAsync(new User());
//        //    _mapperMock.Setup(m => m.Map<UserProfileDto>(It.IsAny<User>())).Returns(new UserProfileDto());

//        //    var result = await _service.UpdateProfile(dto);

//        //    Assert.NotNull(result);
//        //}

//        [Fact]
//        public async Task GetStaffProfileAsync_ReturnsProfile()
//        {
//            var userId = Guid.NewGuid();
//            _repoMock.Setup(r => r.GetUserStaffById(userId)).ReturnsAsync(new User
//            {
//                Id = userId,
//                FullName = "Name",
//                Role = new UserRole { },
//                StaffInfo = new StaffInfo
//                {
//                    SalaryBase = 1000,
//                    Tittle = "T",
//                    Type = "F",
//                    Branch = new Branch { Name = "Branch" }
//                }
//            });

//            var result = await _service.GetStaffProfileAsync(userId);

//            Assert.NotNull(result);
//            Assert.Equal(userId, result.Id);
//        }
//    }
//}
