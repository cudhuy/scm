using Branch.API.Services.Behaviours;
using Domain.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchServiceTest.UnitTests.Repositories
{
    public class ReportRepositoryTests
    {
        private ReportRepository CreateRepository(out SCMDbContext context)
        {
            var options = new DbContextOptionsBuilder<SCMDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            context = new SCMDbContext(options);
            var logger = new LoggerFactory().CreateLogger<ReportRepository>();

            return new ReportRepository(context, logger);
        }

        [Fact]
        public async Task GetTotalCustomers_ShouldReturnCorrectCount()
        {
            var repo = CreateRepository(out var context);

            var userRoleId = Domain.Constanst.AppConstants.USER_ROLE_GUID;
            context.Users.AddRange(
                new User { Id = Guid.NewGuid(), RoleId = userRoleId, IsDeleted = false },
                new User { Id = Guid.NewGuid(), RoleId = userRoleId, IsDeleted = false },
                new User { Id = Guid.NewGuid(), RoleId = Guid.NewGuid(), IsDeleted = false } // Không đúng role
            );
            await context.SaveChangesAsync();

            var result = await repo.GetTotalCustomers();

            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetTotalOrdersForStatus2_ShouldReturnCorrectCount()
        {
            var repo = CreateRepository(out var context);
            var roleId = Domain.Constanst.AppConstants.USER_ROLE_GUID;

            var user = new User { Id = Guid.NewGuid(), RoleId = roleId };
            context.Users.Add(user);

            context.Orders.AddRange(
                new Order { Id = Guid.NewGuid(), Status = 2, IsDeleted = false, User = user, OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest", ShippingAddress = "Unittest" },
                new Order { Id = Guid.NewGuid(), Status = 2, IsDeleted = false, User = user, OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest", ShippingAddress = "Unittest" },
                new Order { Id = Guid.NewGuid(), Status = 1, IsDeleted = false, User = user, OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest", ShippingAddress = "Unittest" } // Không đúng status
            );
            await context.SaveChangesAsync();

            var result = await repo.GetTotalOrdersForStatus2(null);
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetTotalAmountForStatus2Orders_ShouldReturnCorrectSum()
        {
            var repo = CreateRepository(out var context);
            var roleId = Domain.Constanst.AppConstants.USER_ROLE_GUID;

            var user = new User { Id = Guid.NewGuid(), RoleId = roleId };
            context.Users.Add(user);

            context.Orders.AddRange(
                new Order { Id = Guid.NewGuid(), Status = 2, TotalAmount = 100, IsDeleted = false, User = user, OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest", ShippingAddress = "Unittest" },
                new Order { Id = Guid.NewGuid(), Status = 2, TotalAmount = 200, IsDeleted = false, User = user, OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest", ShippingAddress = "Unittest" },
                new Order { Id = Guid.NewGuid(), Status = 1, TotalAmount = 500, IsDeleted = false, User = user, OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest", ShippingAddress = "Unittest" } // Sai status
            );
            await context.SaveChangesAsync();

            var result = await repo.GetTotalAmountForStatus2Orders(null);
            Assert.Equal(300, result);
        }

        //[Fact]
        //public async Task GetTopSellingProducts_ShouldReturnTop5()
        //{
        //    var repo = CreateRepository(out var context);
        //    var roleId = Domain.Constanst.AppConstants.USER_ROLE_GUID;
        //    var user = new User { Id = Guid.NewGuid(), RoleId = roleId };
        //    context.Users.Add(user);

        //    var product1 = new Product { Id = Guid.NewGuid(), Name = "Product A", IsDeleted = false, Description = "UnitTest", Image = "UnitTest" };
        //    var product2 = new Product { Id = Guid.NewGuid(), Name = "Product B", IsDeleted = false, Description = "UnitTest", Image = "UnitTest" };
        //    context.Products.AddRange(product1, product2);

        //    var order = new Order { Id = Guid.NewGuid(), ShippingAddress = "UnitTest", User = user, IsDeleted = false, OrderCode = "UnitTest", OrderType = "UnitTest", RecipientName = "UnitTest", RecipientPhone = "UnitTest" };
        //    context.Orders.Add(order);

        //    context.OrderDetails.AddRange(
        //        new OrderDetail { Order = order, Product = product1, Quantity = 5 },
        //        new OrderDetail { Order = order, Product = product2, Quantity = 10 }
        //    );
        //    await context.SaveChangesAsync();

        //    var result = await repo.GetTopSellingProducts(null);
        //    Assert.Equal(0, result.Count());
        //    Assert.Contains(result, p => p.Id == product2.Id); // product2 bán chạy hơn
        //}

        [Fact]
        public async Task GetLatestFiveOrdersAsync_ShouldReturnLatestOrders()
        {
            var repo = CreateRepository(out var context);
            var roleId = Domain.Constanst.AppConstants.USER_ROLE_GUID;
            var user = new User { Id = Guid.NewGuid(), RoleId = roleId };
            context.Users.Add(user);

            for (int i = 1; i <= 7; i++)
            {
                context.Orders.Add(new Order
                {
                    Id = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow.AddMinutes(-i),
                    User = user,
                    IsDeleted = false,
                    OrderCode = "UnitTest",
                    OrderType = "UnitTest",
                    RecipientPhone = "UnitTest",
                    RecipientName = "UnitTest",
                    ShippingAddress = "UnitTest",
                });
            }

            await context.SaveChangesAsync();
            var result = await repo.GetLatestFiveOrdersAsync(null);

            Assert.Equal(5, result.Count);
        }
    }
}
