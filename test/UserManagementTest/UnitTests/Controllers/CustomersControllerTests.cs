using AutoMapper;
using Castle.Core.Resource;
using Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading.Tasks;
using UserManagement.API.Controllers;
using UserManagement.API.Dtos.Customer;
using UserManagement.API.Dtos.Profile;
using UserManagement.API.Services.Interfaces;
using Xunit;

namespace UserManagementTest.UnitTests.Controllers
{
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomerService> _customerServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _customerServiceMock = new Mock<ICustomerService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new CustomersController(_customerServiceMock.Object, _mapperMock.Object);
        }

        // --- GetAllCustomers ---
        [Fact]
        public async Task GetAllCustomers_ShouldReturnOk_WhenCustomersExist()
        {
            var customers = new GetCustomersResponseDto
            {
                Items = new List<CustomerDto> {
                    new CustomerDto {
                        Id = Guid.NewGuid(), FullName = "John Doe"
                    }
                },
                TotalItems = 1,
                CurrentPage = 1,
                PageSize = 10,
                TotalPages = 1
            };

            _customerServiceMock.Setup(s => s.GetAllCustomers(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(customers);

            var result = await _controller.GetAllCustomers();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(customers);
        }

        [Fact]
        public async Task GetAllCustomers_ShouldReturnEmptyList_WhenNoCustomersExist()
        {
            // Arrange
            var emptyResponse = new GetCustomersResponseDto
            {
                Items = new List<CustomerDto>(), // Danh sách rỗng thay vì null
                TotalItems = 0,
                CurrentPage = 0,
                PageSize = 0,
                TotalPages = 0
            };

            _customerServiceMock.Setup(s => s.GetAllCustomers(It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(emptyResponse);

            // Act
            var result = await _controller.GetAllCustomers();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedList = okResult.Value as GetCustomersResponseDto;

            returnedList.Should().NotBeNull();
            returnedList.Items.Should().BeEmpty(); // Đảm bảo danh sách rỗng
        }


        // --- GetCustomerById ---
        [Fact]
        public async Task GetCustomerById_ShouldReturnOk_WhenCustomerExists()
        {
            var customer = new User { Id = Guid.NewGuid(), FullName = "John Doe", CreatedAt = DateTime.UtcNow };
            _customerServiceMock.Setup(s => s.GetCustomerById(It.IsAny<Guid>())).ReturnsAsync(customer);
            _mapperMock.Setup(m => m.Map<CustomerDto>(customer)).Returns(new CustomerDto { Id = customer.Id, FullName = customer.FullName });

            var result = await _controller.GetCustomerById(customer.Id);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task GetCustomerById_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            _customerServiceMock.Setup(s => s.GetCustomerById(It.IsAny<Guid>())).ReturnsAsync((User)null);

            var result = await _controller.GetCustomerById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        // --- UpdateCustomer ---
        [Fact]
        public async Task UpdateCustomer_ShouldReturnOk_WhenUpdateIsSuccessful()
        {
            var updateDto = new CustomerUpdateDto { Id = Guid.NewGuid(), Status = "Updated Name" };
            _customerServiceMock.Setup(s => s.UpdateCustomer(updateDto)).ReturnsAsync(true);

            var result = await _controller.UpdateCustomer(updateDto);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(new { message = "Cập nhật thành công." });
        }

        [Fact]
        public async Task UpdateCustomer_ShouldReturnBadRequest_WhenUpdateFails()
        {
            var updateDto = new CustomerUpdateDto { Id = Guid.NewGuid(), Status = "Updated Name" };
            _customerServiceMock.Setup(s => s.UpdateCustomer(updateDto)).ReturnsAsync(false);

            var result = await _controller.UpdateCustomer(updateDto);

            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().BeEquivalentTo(new { message = "Cập nhật thất bại." });
        }

        [Fact]
        public async Task UpdateCustomer_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            _controller.ModelState.AddModelError("FullName", "FullName is required");

            var result = await _controller.UpdateCustomer(new CustomerUpdateDto());

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        // --- GetProfileCustomerById ---
        [Fact]
        public async Task GetProfileCustomerById_ShouldReturnOk_WhenCustomerExists()
        {
            var customer = new User { Id = Guid.NewGuid(), FullName = "John Doe", CreatedAt = DateTime.UtcNow };
            _customerServiceMock.Setup(s => s.GetCustomerById(It.IsAny<Guid>())).ReturnsAsync(customer);
            _mapperMock.Setup(m => m.Map<CustomerDto>(customer)).Returns(new CustomerDto { Id = customer.Id, FullName = customer.FullName });

            var result = await _controller.GetProfileCustomerById(customer.Id);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task GetProfileCustomerById_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            _customerServiceMock.Setup(s => s.GetCustomerById(It.IsAny<Guid>())).ReturnsAsync((User)null);

            var result = await _controller.GetProfileCustomerById(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        // --- GetUserProfile ---
        [Fact]
        public async Task GetUserProfile_ShouldReturnOk_WhenUserProfileExists()
        {
            var userProfile = new UserProfileDto { Id = Guid.NewGuid(), FullName = "User Name" };
            _customerServiceMock.Setup(s => s.GetUserProfileAsync(It.IsAny<Guid>())).ReturnsAsync(userProfile);

            var result = await _controller.GetUserProfile(userProfile.Id);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(userProfile);
        }

        [Fact]
        public async Task GetUserProfile_ShouldReturnNotFound_WhenUserProfileDoesNotExist()
        {
            _customerServiceMock.Setup(s => s.GetUserProfileAsync(It.IsAny<Guid>())).ReturnsAsync((UserProfileDto)null);

            var result = await _controller.GetUserProfile(Guid.NewGuid());

            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().BeEquivalentTo(new { message = "Không tìm thấy người dùng." });
        }
    }
}
