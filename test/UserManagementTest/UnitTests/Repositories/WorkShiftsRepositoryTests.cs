using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagementTest.UnitTests.Repositories
{
    using Domain.Context;
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UserManagement.API.Repositories.Behaviours;
    using Xunit;
    using System.Linq;

        public class WorkShiftsRepositoryTests
        {
            private SCMDbContext GetDbContext()
            {
                var options = new DbContextOptionsBuilder<SCMDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options;
                return new SCMDbContext(options);
            }

            private WorkShiftsRepository GetRepository(SCMDbContext context)
            {
                var logger = new LoggerFactory().CreateLogger<WorkShiftsRepository>();
                return new WorkShiftsRepository(logger, context);
            }

            [Fact]
            public async Task Save_ShouldReturnTrue_WhenChangesSaved()
            {
                var context = GetDbContext();
                context.WorkShifts.Add(new WorkShift { Id = Guid.NewGuid(), Name = "Shift A", StartTime = TimeSpan.MinValue, EndTime = TimeSpan.MaxValue });
                var repo = GetRepository(context);

                var result = await repo.Save();

                Assert.True(result);
            }

            [Fact]
            public async Task GetShifts_ShouldReturnShifts_WhenExists()
            {
                var context = GetDbContext();
                context.WorkShifts.Add(new WorkShift { Id = Guid.NewGuid(), Name = "Shift B", StartTime = TimeSpan.MinValue, EndTime = TimeSpan.MaxValue, IsDeleted = false });
                context.WorkShifts.Add(new WorkShift { Id = Guid.NewGuid(), Name = "Deleted Shift", IsDeleted = true });
                context.SaveChanges();

                var repo = GetRepository(context);
                var result = await repo.GetShifts();

                Assert.Single(result);
                Assert.Equal("Shift B", result.First().Name);
            }

            [Fact]
            public async Task GetShiftById_ShouldReturnCorrectShift_WhenExists()
            {
                var context = GetDbContext();
                var shiftId = Guid.NewGuid();
                context.WorkShifts.Add(new WorkShift { Id = shiftId, Name = "Shift C" });
                context.SaveChanges();

                var repo = GetRepository(context);
                var result = await repo.GetShiftById(shiftId);

                Assert.NotNull(result);
                Assert.Equal("Shift C", result.Name);
            }

            [Fact]
            public async Task GetShiftById_ShouldReturnNull_WhenNotExists()
            {
                var context = GetDbContext();
                var repo = GetRepository(context);

                var result = await repo.GetShiftById(Guid.NewGuid());

                Assert.Null(result);
            }
        }
    }

