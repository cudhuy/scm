using Branch.API.Controllers;
using Branch.API.Dtos.Order;
using Branch.API.Dtos.Report;
using Branch.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BranchServiceTest.UnitTests.Controllers
{
    public class ReportControllerTests
    {
        private readonly Mock<IReportService> _mockReportService;
        private readonly ReportController _controller;

        public ReportControllerTests()
        {
            _mockReportService = new Mock<IReportService>();
            _controller = new ReportController(_mockReportService.Object);
        }

        [Fact]
        public async Task GetRevenueReport_ReturnsBadRequest_WhenYearIsZero()
        {
            var result = await _controller.GetRevenueReport(null, null, 0);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Tham số năm là bắt buộc.", badRequest.Value);
        }

        //[Fact]
        //public async Task GetRevenueReport_ReturnsOk_WhenValid()
        //{
        //    _mockReportService.Setup(x => x.GetRevenueReportAsync(null, null, 2024))
        //        .ReturnsAsync(new RevenueReportDto());

        //    var result = await _controller.GetRevenueReport(null, null, 2024);

        //    var ok = Assert.IsType<OkObjectResult>(result);
        //    Assert.Equal(new RevenueReportDto(), ok.Value);
        //}

        [Fact]
        public async Task GetOrderStatistics_ReturnsBadRequest_ForInvalidWeekMonthYear()
        {
            var resultWeek = await _controller.GetOrderStatistics(null, 0, 1, 2024);
            Assert.IsType<BadRequestObjectResult>(resultWeek);

            var resultMonth = await _controller.GetOrderStatistics(null, 1, 13, 2024);
            Assert.IsType<BadRequestObjectResult>(resultMonth);

            var resultYear = await _controller.GetOrderStatistics(null, 1, 1, 1999);
            Assert.IsType<BadRequestObjectResult>(resultYear);
        }

        //[Fact]
        //public async Task GetOrderStatistics_ReturnsOk_WhenValid()
        //{
        //    _mockReportService.Setup(x => x.GetOrderStatistics(null, 1, 1, 2024)).ReturnsAsync(new OrderStatisticsResponseDto());
        //    var result = await _controller.GetOrderStatistics(null, 1, 1, 2024);
        //    var ok = Assert.IsType<OkObjectResult>(result);
        //    Assert.Equal(new OrderStatisticsResponseDto(), ok.Value);
        //}

        [Fact]
        public async Task GetReportSummary_ReturnsOk_WhenDataExists()
        {
            _mockReportService.Setup(x => x.GetTotalCustomers()).ReturnsAsync(5);
            _mockReportService.Setup(x => x.GetTotalAmountForStatus2Orders(null)).ReturnsAsync(1000);
            _mockReportService.Setup(x => x.GetTotalOrdersForStatus2(null)).ReturnsAsync(3);
            _mockReportService.Setup(x => x.GetTotalProductsCount()).ReturnsAsync(7);

            var result = await _controller.GetReportSummary();
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        [Fact]
        public async Task GetLatestFiveOrders_ReturnsNotFound_WhenNoOrders()
        {
            _mockReportService.Setup(x => x.GetLatestFiveOrders(null)).ReturnsAsync(new List<OrderDto>());
            var result = await _controller.GetLatestFiveOrders(null);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetLatestFiveOrders_ReturnsOk_WhenOrdersExist()
        {
            _mockReportService.Setup(x => x.GetLatestFiveOrders(null)).ReturnsAsync(new List<OrderDto> { new OrderDto() });
            var result = await _controller.GetLatestFiveOrders(null);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(ok.Value);
        }

        //[Fact]
        //public async Task GetOrderById_ReturnsOk_WhenFound()
        //{
        //    var id = Guid.NewGuid();
        //    _mockReportService.Setup(x => x.GetOrderByIdAsync(id)).ReturnsAsync(new OrderDetailDto());
        //    var result = await _controller.GetOrderById(id);
        //    var ok = Assert.IsType<OkObjectResult>(result);
        //    Assert.Equal(new OrderDetailDto(), ok.Value);
        //}

        [Fact]
        public async Task GetOrderById_ReturnsNotFound_WhenKeyNotFound()
        {
            var id = Guid.NewGuid();
            _mockReportService.Setup(x => x.GetOrderByIdAsync(id)).ThrowsAsync(new KeyNotFoundException("Không tìm thấy"));
            var result = await _controller.GetOrderById(id);
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Không tìm thấy", notFound.Value.ToString());
        }

        [Fact]
        public async Task GetTopSellingProducts_ReturnsOk_WhenProductsExist()
        {
            _mockReportService.Setup(x => x.GetTopSellingProductsAsync(null)).ReturnsAsync(new List<ProductBestSellingDto> { new ProductBestSellingDto() });
            var result = await _controller.GetTopSellingProducts(null);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetTopSellingProducts_ReturnsNotFound_WhenEmpty()
        {
            _mockReportService.Setup(x => x.GetTopSellingProductsAsync(null)).ReturnsAsync(new List<ProductBestSellingDto>());
            var result = await _controller.GetTopSellingProducts(null);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetRecentCustomers_ReturnsOk_WhenExist()
        {
            _mockReportService.Setup(x => x.GetRecentCustomers(null)).ReturnsAsync(new List<RecentCustomerDto> { new RecentCustomerDto() });
            var result = await _controller.GetRecentCustomers(null);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetRecentCustomers_ReturnsNotFound_WhenEmpty()
        {
            _mockReportService.Setup(x => x.GetRecentCustomers(null)).ReturnsAsync(new List<RecentCustomerDto>());
            var result = await _controller.GetRecentCustomers(null);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetDetailProduct_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();
            _mockReportService.Setup(x => x.GetDetailProductsAsync(id)).ReturnsAsync(new ProductDetailDto());
            var result = await _controller.GetDetailProduct(id);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetDetailProduct_ReturnsNotFound_WhenNull()
        {
            var id = Guid.NewGuid();
            _mockReportService.Setup(x => x.GetDetailProductsAsync(id)).ReturnsAsync((ProductDetailDto) null);
            var result = await _controller.GetDetailProduct(id);
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetDetailUser_ReturnsOk_WhenFound()
        {
            var id = Guid.NewGuid();
            _mockReportService.Setup(x => x.GetDetailUserAsync(id)).ReturnsAsync(new UserDetailDto());
            var result = await _controller.GetDetailUser(id);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetDetailUser_ReturnsNotFound_WhenNull()
        {
            var id = Guid.NewGuid();
            _mockReportService.Setup(x => x.GetDetailUserAsync(id)).ReturnsAsync((UserDetailDto)null);
            var result = await _controller.GetDetailUser(id);
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
