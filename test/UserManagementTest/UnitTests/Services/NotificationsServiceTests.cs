//using AutoMapper;
//using Microsoft.Extensions.Logging;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UserManagement.API.Dtos.Notification;
//using UserManagement.API.Repositories.Behaviours;
//using UserManagement.API.Repositories.Interfaces;
//using UserManagement.API.Services.Behaviours;

//namespace UserManagementTest.UnitTests.Services
//{
//    public class NotificationsServiceTests
//    {
//        private readonly Mock<INotificationRepository> _notificationRepoMock;
//        private readonly Mock<IKafkaProducer> _kafkaProducerMock;
//        private readonly Mock<IMailService> _mailServiceMock;
//        private readonly NotificationsService _service;

//        public NotificationsServiceTests()
//        {
//            _notificationRepoMock = new Mock<INotificationRepository>();
//            _kafkaProducerMock = new Mock<IKafkaProducer>();
//            _mailServiceMock = new Mock<IMailService>();
//            _service = new NotificationsService(
//                _notificationRepoMock.Object,
//                _kafkaProducerMock.Object,
//                _mailServiceMock.Object
//            );
//        }

//        [Fact]
//        public async Task GetAllNotificationSystem_ShouldReturnList()
//        {
//            var expected = new List<NotificationSystemDTO> { new NotificationSystemDTO { Id = 1 } };
//            _notificationRepoMock.Setup(r => r.GetAllNotificationSystem()).ReturnsAsync(expected);

//            var result = await _service.GetAllNotificationSystem();

//            Assert.Equal(expected, result);
//        }

//        [Fact]
//        public async Task GetAllNotificationUsers_ShouldReturnList()
//        {
//            var expected = new List<NotificationUserDTO> { new NotificationUserDTO { Id = 1 } };
//            _notificationRepoMock.Setup(r => r.GetAllNotificationUsers()).ReturnsAsync(expected);

//            var result = await _service.GetAllNotificationUsers();

//            Assert.Equal(expected, result);
//        }

//        [Fact]
//        public async Task DeleteNotificationSystem_ShouldDeleteCorrectly()
//        {
//            int id = 1;
//            _notificationRepoMock.Setup(r => r.DeleteNotificationSystem(id)).Returns(Task.CompletedTask);

//            await _service.DeleteNotificationSystem(id);

//            _notificationRepoMock.Verify(r => r.DeleteNotificationSystem(id), Times.Once);
//        }

//        [Fact]
//        public async Task UpdateNotificationSystem_ShouldUpdateSuccessfully()
//        {
//            var input = new NotificationSystemUpdateDTO { Id = 1 };
//            var updated = new NotificationSystemDTO { Id = 1 };
//            _notificationRepoMock.Setup(r => r.UpdateNotificationSystem(input)).ReturnsAsync(updated);

//            var result = await _service.UpdateNotificationSystem(input);

//            Assert.Equal(updated.Id, result.Id);
//        }

//        [Fact]
//        public async Task CreateNotificationSystem_ShouldCallKafkaAndReturnNotification()
//        {
//            var input = new NotificationSystemCreateDTO { Title = "Test" };
//            var notification = new NotificationSystemDTO { Id = 1, Title = "Test" };
//            _notificationRepoMock.Setup(r => r.CreateNotificationSystem(input)).ReturnsAsync(notification);

//            var result = await _service.CreateNotificationSystem(input);

//            Assert.Equal("Test", result.Title);
//            _kafkaProducerMock.Verify(k => k.SendMessageAsync("NotificationSystem", It.IsAny<object>()), Times.Once);
//        }

//        [Fact]
//        public async Task CreateNotificationUser_ShouldCallKafkaAndReturnNotification()
//        {
//            var input = new NotificationUserCreateDTO { Title = "User Notification" };
//            var notification = new NotificationUserDTO { Id = 2, Title = "User Notification" };
//            _notificationRepoMock.Setup(r => r.CreateNotificationUser(input)).ReturnsAsync(notification);

//            var result = await _service.CreateNotificationUser(input);

//            Assert.Equal(2, result.Id);
//            _kafkaProducerMock.Verify(k => k.SendMessageAsync("NotificationUser", It.IsAny<object>()), Times.Once);
//        }

//        [Fact]
//        public async Task CreateNotificationForRanks_ShouldSendEmailsAndCreateNotifications()
//        {
//            var input = new NotificationRankCreateDTO
//            {
//                Title = "Title",
//                Content = "Content",
//                Ranks = new List<string> { "Gold", "Silver" }
//            };
//            var users = new List<UserDTO> {
//            new UserDTO { Id = 1, Email = "a@mail.com" },
//            new UserDTO { Id = 2, Email = "b@mail.com" }
//        };
//            _notificationRepoMock.Setup(r => r.GetUsersByRank(input.Ranks)).ReturnsAsync(users);
//            _notificationRepoMock.Setup(r => r.CreateNotificationForRanks(It.IsAny<List<NotificationRank>>())).Returns(Task.CompletedTask);

//            await _service.CreateNotificationForRanks(input);

//            _mailServiceMock.Verify(m => m.SendEmail("a@mail.com", input.Title, input.Content), Times.Once);
//            _mailServiceMock.Verify(m => m.SendEmail("b@mail.com", input.Title, input.Content), Times.Once);
//            _notificationRepoMock.Verify(r => r.CreateNotificationForRanks(It.IsAny<List<NotificationRank>>()), Times.Once);
//        }

//        [Fact]
//        public async Task GetAllNotificationRanksForPage_ShouldReturnPagedResult()
//        {
//            var expected = new PageResultDTO<NotificationRankDTO>
//            {
//                Items = new List<NotificationRankDTO> { new NotificationRankDTO { Id = 1 } },
//                TotalItems = 1
//            };
//            _notificationRepoMock.Setup(r => r.GetAllNotificationRanksForPage(1, 10)).ReturnsAsync(expected);

//            var result = await _service.GetAllNotificationRanksForPage(1, 10);

//            Assert.Single(result.Items);
//            Assert.Equal(1, result.TotalItems);
//        }
//    }
//}
