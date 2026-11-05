using Domain.Context;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagement.API.Repositories.Behaviours;

namespace UserManagementTest.UnitTests.Repositories
{
    public class NotificationsRepositoryTests
    {
        private SCMDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<SCMDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SCMDbContext(options);
        }

        private NotificationsRepository GetRepository(SCMDbContext context)
        {
            var logger = new Mock<ILogger<NotificationsRepository>>().Object;
            return new NotificationsRepository(context, logger);
        }

        [Fact]
        public async Task GetListNotificationSystem_ReturnsSystemNotifications()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetRepository(context);

            context.Notifications.Add(new Notification
            {
                Id = Guid.NewGuid(),
                Title = "Test",
                Type = (int)NotificationEnum.EType.System,
                IsDeleted = false,
                CreatedAt = DateTime.Now,
                Content = "UnitTest"
            });
            await context.SaveChangesAsync();

            var result = await repo.GetListNotificationSystem();

            Assert.Single(result);
            Assert.Equal((int)NotificationEnum.EType.System, result.First().Type);
        }

        [Fact]
        public async Task CreateNotificationSystem_AddsNotification()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetRepository(context);

            var notification = new Notification { Title = "Test", Content = "Content" };
            var result = await repo.CreateNotificationSystem(notification);

            Assert.True(result);
            Assert.Single(context.Notifications);
        }

        [Fact]
        public async Task DeletetNotificationSystem_DeletesNotification()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetRepository(context);

            var noti = new Notification { Id = Guid.NewGuid(), Title = "Test", IsDeleted = false, Content = "UnitTest" };
            context.Notifications.Add(noti);
            await context.SaveChangesAsync();

            var result = await repo.DeletetNotificationSystem(noti.Id);
            var updated = await context.Notifications.FindAsync(noti.Id);

            Assert.True(result);
            Assert.True(updated.IsDeleted);
        }

        [Fact]
        public async Task UpdateNotificationSystem_UpdatesContent()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetRepository(context);

            var noti = new Notification { Id = Guid.NewGuid(), Title = "Old", Content = "Old Content" };
            context.Notifications.Add(noti);
            await context.SaveChangesAsync();

            noti.Content = "New Content";
            var result = await repo.UpdateNotificationSystem(noti.Id, noti);

            Assert.True(result);
            Assert.Equal("New Content", (await context.Notifications.FindAsync(noti.Id)).Content);
        }

        [Fact]
        public async Task GetNotificationSystemById_ReturnsCorrectNotification()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetRepository(context);

            var noti = new Notification { Id = Guid.NewGuid(), Title = "Test", Content = "UnitTest" };
            context.Notifications.Add(noti);
            await context.SaveChangesAsync();

            var result = await repo.GetNotificationSystemById(noti.Id);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Title);
        }

        [Fact]
        public async Task GetNotificationsCountToday_ReturnsCorrectCount()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetRepository(context);

            context.Notifications.Add(new Notification
            {
                Id = Guid.NewGuid(),
                Title = "Today",
                CreatedAt = DateTime.Now,
                IsDeleted = false,
                Content = "UnitTest"
            });
            await context.SaveChangesAsync();

            var count = await repo.GetNotificationsCountToday();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetNotificationCounts_ReturnsCorrectCounts()
        {
            using var context = GetInMemoryDbContext();
            var repo = GetRepository(context);

            context.Notifications.AddRange(new[]
            {
            new Notification { Id = Guid.NewGuid(), Type = (int)NotificationEnum.EType.User, IsDeleted = false, Content = "UnitTest", Title = " UnitTest" },
            new Notification { Id = Guid.NewGuid(), Type = (int)NotificationEnum.EType.System, IsDeleted = false, Content = "UnitTest", Title = " UnitTest" },
            new Notification { Id = Guid.NewGuid(), Type = (int)NotificationEnum.EType.System, IsDeleted = true, Content = "UnitTest", Title = " UnitTest" }
        });
            await context.SaveChangesAsync();

            var (userCount, systemCount) = await repo.GetNotificationCounts();

            Assert.Equal(1, userCount);
            Assert.Equal(1, systemCount);
        }
    }
}
