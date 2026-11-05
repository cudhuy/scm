//using AutoMapper;
//using Domain.Constanst;
//using Domain.Context;
//using Domain.Entities;
//using Microsoft.EntityFrameworkCore;
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
//using UserManagement.API.Messaging.Producers;
//using UserManagement.API.Repositories.Behaviours;

//namespace UserManagementTest.UnitTests.Repositories
//{
//    public class StaffInformationRepositoryTests
//    {
//        private readonly StaffInformationRepository _repository;
//        private readonly SCMDbContext _context;
//        private readonly Mock<IMapper> _mapper;

//        public StaffInformationRepositoryTests()
//        {
//            var options = new DbContextOptionsBuilder<SCMDbContext>()
//                .UseInMemoryDatabase(Guid.NewGuid().ToString())
//                .Options;

//            _context = new SCMDbContext(options);

//            var logger = new Mock<ILogger<StaffInformationRepository>>();
//            _mapper = new Mock<IMapper>();
//            var redisHelper = new Mock<RedisHelper>();
//            var emailHelper = new Mock<EmailHelper>();
//            var createProducer = new Mock<CreateStaffProducerService>();
//            var updateProducer = new Mock<UpdateProfileProducerService>();

//            _repository = new StaffInformationRepository(
//                _context,
//                logger.Object,
//                redisHelper.Object,
//                _mapper.Object,
//                emailHelper.Object,
//                createProducer.Object,
//                updateProducer.Object
//            );
//        }

//        [Fact]
//        public async Task CreateStaff_ShouldReturnFalse_WhenInputIsNull()
//        {
//            var result = await _repository.CreateStaff(null);
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task CreateStaff_ShouldReturnTrue_WhenValid()
//        {
//            var bracnchId = Guid.NewGuid();
//            var vm = new CreateStaffDto
//            {
//                BranchId = bracnchId,
//                Title = "Nhân viên",
//                Email = "UnitTest",
//                IsStaff = true
//            };

//            _mapper.Setup(m => m.Map<StaffInfo>(It.IsAny<CreateStaffDto>()))
//                   .Returns(new StaffInfo { UserId = bracnchId, Tittle = "Nhân viên" });

//            var result = await _repository.CreateStaff(vm);

//            Assert.True(result);
//            var added = await _context.StaffInfos.FindAsync(bracnchId);
//            Assert.NotNull(added);
//            Assert.Equal("Nhân viên", added.Tittle);
//        }

//        [Fact]
//        public async Task DeleteStaff_ShouldReturnFalse_WhenNotExists()
//        {
//            var result = await _repository.DeleteStaff(Guid.NewGuid());
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task DeleteStaff_ShouldSetIsDeletedTrue_WhenExists()
//        {
//            var user = new User
//            {
//                Id = Guid.NewGuid(),
//                Email = "test@a.com",
//                RoleId = AppConstants.STAFF_ROLE_GUID,
//                IsDeleted = false
//            };
//            await _context.Users.AddAsync(user);
//            await _context.SaveChangesAsync();

//            var result = await _repository.DeleteStaff(user.Id);
//            var deleted = await _context.Users.FindAsync(user.Id);

//            Assert.True(result);
//            Assert.True(deleted.IsDeleted);
//        }

//        [Fact]
//        public async Task GetListStaffInfor_ShouldReturnEmpty_WhenNoData()
//        {
//            var result = await _repository.GetListStaffInfor();
//            Assert.Empty(result);
//        }

//        [Fact]
//        public async Task GetListStaffInfor_ShouldReturnList_WhenDataExists()
//        {
//            var user = new User
//            {
//                Id = Guid.NewGuid(),
//                Email = "test@staff.com",
//                RoleId = AppConstants.STAFF_ROLE_GUID,
//                IsDeleted = false
//            };
//            await _context.Users.AddAsync(user);
//            await _context.StaffInfos.AddAsync(new StaffInfo
//            {
//                UserId = user.Id,
//                Tittle = "Test Staff"
//            });
//            await _context.SaveChangesAsync();

//            _mapper.Setup(m => m.Map<StaffInfo>(It.IsAny<StaffInfo>()))
//                   .Returns(new StaffInfo { UserId = user.Id, Tittle = "Test Staff" });

//            var result = await _repository.GetListStaffInfor();

//            Assert.Single(result);
//            Assert.Equal("Test Staff", result.First().Tittle);
//        }

//        [Fact]
//        public async Task GetByUserId_ShouldReturnCorrectStaff()
//        {
//            var id = Guid.NewGuid();
//            await _context.Users.AddAsync(new User
//            {
//                Id = id,
//                Email = "staff@x.com",
//                RoleId = AppConstants.STAFF_ROLE_GUID,
//                IsDeleted = false
//            });
//            await _context.StaffInfos.AddAsync(new StaffInfo
//            {
//                UserId = id,
//                Tittle = "Nhân viên"
//            });
//            await _context.SaveChangesAsync();

//            var result = await _repository.GetByUserId(id);
//            Assert.NotNull(result);
//            Assert.Equal("Nhân viên", result.Tittle);
//        }

//        [Fact]
//        public async Task GetUserStaffById_ShouldReturnNull_WhenNotExists()
//        {
//            var result = await _repository.GetUserStaffById(Guid.NewGuid());
//            Assert.Null(result);
//        }

//        [Fact]
//        public async Task GetUserStaffById_ShouldReturnUser_WhenExists()
//        {
//            var id = Guid.NewGuid();
//            var user = new User
//            {
//                Id = id,
//                Email = "user@staff.com",
//                RoleId = AppConstants.STAFF_ROLE_GUID
//            };
//            await _context.Users.AddAsync(user);
//            await _context.SaveChangesAsync();

//            _mapper.Setup(m => m.Map<StaffInfo>(user))
//                   .Returns(new StaffInfo { UserId = id});

//            var result = await _repository.GetUserStaffById(id);

//            Assert.NotNull(result);
//            Assert.Equal(user.Email, result.Email);
//        }

//        [Fact]
//        public async Task UpdateStaff_ShouldReturnFalse_WhenInputNull()
//        {
//            var result = await _repository.UpdateStaff(null);
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task UpdateStaff_ShouldReturnFalse_WhenNotFound()
//        {
//            var result = await _repository.UpdateStaff(new UpdateStaffDto { UserId = Guid.NewGuid() });
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task UpdateStaff_ShouldReturnTrue_WhenValid()
//        {
//            var id = Guid.NewGuid();
//            await _context.StaffInfos.AddAsync(new StaffInfo
//            {
//                UserId = id,
//                Tittle = "Old Title"
//            });
//            await _context.SaveChangesAsync();

//            var vm = new UpdateStaffDto
//            {
//                UserId = id,
//                Title = "New Title"
//            };

//            var result = await _repository.UpdateStaff(vm);
//            var updated = await _context.StaffInfos.FindAsync(id);

//            Assert.True(result);
//            Assert.Equal("New Title", updated.Tittle);
//        }

//        [Fact]
//        public async Task UpdateProfile_ShouldReturnFalse_WhenInputNull()
//        {
//            var dto = new UpdateProfileDto();
//            var result = await _repository.UpdateProfile(dto, null);
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task UpdateProfile_ShouldReturnFalse_WhenUserNotFound()
//        {
//            var result = await _repository.UpdateProfile(new UpdateProfileDto { UserId = Guid.NewGuid() }, null);
//            Assert.False(result);
//        }

//        [Fact]
//        public async Task UpdateProfile_ShouldReturnTrue_WhenValid()
//        {
//            var id = Guid.NewGuid();
//            await _context.Users.AddAsync(new User
//            {
//                Id = id,
//                Email = "user@update.com"
//            });
//            await _context.StaffInfos.AddAsync(new StaffInfo
//            {
//                UserId = id,
//                Tittle = "Cũ"
//            });
//            await _context.SaveChangesAsync();

//            var vm = new UpdateProfileDto
//            {
//                UserId = id,
//                Address = "UnitTest",
//                FullName = "UnitTest"
//            };

//            var result = await _repository.UpdateProfile(vm, null);
//            var updated = await _context.StaffInfos.FindAsync(id);

//            Assert.True(result);
//            Assert.Equal("UnitTest", updated.Tittle);
//        }
//    }
//}
