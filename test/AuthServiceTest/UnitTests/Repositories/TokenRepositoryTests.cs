using Auth.API.Entities;
using Auth.API.Repositories.Behaviours;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Moq;

namespace AuthServiceTest.UnitTests.Repositories
{
    public class TokenRepositoryTests
    {
        private readonly Mock<IMongoCollection<Token>> _mockTokenCollection;
        private readonly Mock<IMongoDatabase> _mockDatabase;
        private readonly Mock<ILogger<TokenRepository>> _mockLogger;
        private readonly TokenRepository _tokenRepository;

        public TokenRepositoryTests()
        {
            _mockTokenCollection = new Mock<IMongoCollection<Token>>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockLogger = new Mock<ILogger<TokenRepository>>();

            _mockDatabase.Setup(db => db.GetCollection<Token>("token", null))
                .Returns(_mockTokenCollection.Object);

            _tokenRepository = new TokenRepository(_mockDatabase.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task DeleteOtpByUserId_ShouldReturnTrue_WhenTokenExists()
        {
            _mockTokenCollection
                .Setup(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Token>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteResult.Acknowledged(1));

            var result = await _tokenRepository.DeleteOtpByUserId("user123");

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteOtpByUserId_ShouldReturnFalse_WhenTokenNotExists()
        {
            _mockTokenCollection
                .Setup(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Token>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DeleteResult.Acknowledged(0));

            var result = await _tokenRepository.DeleteOtpByUserId("user123");

            Assert.False(result);
        }

        //[Fact]
        //public async Task GetOtpByUserId_ShouldReturnTokenValue_WhenTokenExists()
        //{
        //    var expectedToken = new Token { UserId = "user123", TokenValue = "otp123", TokenType = "OTP" };

        //    var mockCursor = new Mock<IAsyncCursor<Token>>();
        //    mockCursor.Setup(c => c.Current).Returns(new List<Token> { expectedToken });
        //    mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);

        //    _mockTokenCollection
        //        .Setup(c => c.FindSync(It.IsAny<FilterDefinition<Token>>(), It.IsAny<FindOptions<Token, Token>>(), It.IsAny<CancellationToken>()))
        //        .Returns(mockCursor.Object);

        //    var result = await _tokenRepository.GetOtpByUserId("user123");

        //    Assert.Equal("otp123", result);
        //}

        //[Fact]
        //public async Task GetOtpByUserId_ShouldReturnNull_WhenTokenNotExists()
        //{
        //    var mockCursor = new Mock<IAsyncCursor<Token>>();
        //    mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(false);

        //    _mockTokenCollection
        //        .Setup(c => c.FindSync(It.IsAny<FilterDefinition<Token>>(), It.IsAny<FindOptions<Token, Token>>(), It.IsAny<CancellationToken>()))
        //        .Returns(mockCursor.Object);

        //    var result = await _tokenRepository.GetOtpByUserId("user123");

        //    Assert.Null(result);
        //}

        [Fact]
        public async Task SaveTokenToDB_ShouldInsertTokenSuccessfully()
        {
            _mockTokenCollection.Setup(c => c.InsertOneAsync(It.IsAny<Token>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _tokenRepository.SaveTokenToDB("user123", "test_token", "OTP");

            Assert.True(result);
        }

        [Fact]
        public async Task SaveTokenToDB_ShouldThrowException_WhenTokenIsEmpty()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _tokenRepository.SaveTokenToDB("user123", "", "OTP"));
        }
    }
}
