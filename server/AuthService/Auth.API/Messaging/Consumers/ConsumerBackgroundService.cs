using Auth.API.Entities;
using Auth.API.Messaging.KafkaMessage;
using Confluent.Kafka;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Auth.API.Messaging.Consumers
{
    public class ConsumerBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ConsumerBackgroundService> _logger;
        private readonly KafkaSettings _kafkaSettings;
        private readonly ConsumerConfig _config;
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<StaffInfo> _staffInfosCollection;

        public ConsumerBackgroundService(IServiceProvider serviceProvider, ILogger<ConsumerBackgroundService> logger, KafkaSettings kafkaSettings, IMongoDatabase database)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _kafkaSettings = kafkaSettings;
            _config = new ConsumerConfig
            {
                GroupId = _kafkaSettings.GroupId,
                BootstrapServers = _kafkaSettings.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Latest,
                EnableAutoCommit = false
            };

            _usersCollection = database.GetCollection<User>("user");
            _staffInfosCollection = database.GetCollection<StaffInfo>("staffInfos");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Entered method ExecuteAsync in ConsumerBackgroundService");
            return Task.Run(async () =>
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(_config).Build();
                consumer.Subscribe(_kafkaSettings.ConsumerTopics);

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        var consumeResult = consumer.Consume(stoppingToken);
                        await ProcessMessage(consumeResult.Topic, consumeResult.Message.Value);
                        _logger.LogInformation($"Received message: {consumeResult.Message.Value}");
                        consumer.Commit(consumeResult);
                    }
                }
                catch (OperationCanceledException)
                {
                    consumer.Close();
                }
                _logger.LogInformation("Leaving method ExecuteAsync in ConsumerBackgroundService");
            }, stoppingToken);
        }

        private async Task ProcessMessage(string topic, string message)
        {
            _logger.LogInformation("Entered method ProcessMessage in ConsumerBackgroundService");
            switch (topic)
            {
                case "create-staff-topic":
                    _logger.LogInformation("Case create-staff-topic");
                    var staffCreated = JsonConvert.DeserializeObject<StaffCreatedMessage>(message);
                    if (staffCreated != null)
                        await CreateUserAndStaffInfo(staffCreated);
                    break;
                case "update-profile-topic":
                    _logger.LogInformation("Case update-profile-topic");
                    var profileUpdated = JsonConvert.DeserializeObject<UpdateProfileMessage>(message);
                    if (profileUpdated != null)
                        await UpdateProfile(profileUpdated);
                    break;
                default:
                    _logger.LogWarning($"Received message from unknown topic: {topic}");
                    return;
            }
            _logger.LogInformation("Leaving method ProcessMessage in ConsumerBackgroundService");
        }

        private async Task UpdateProfile(UpdateProfileMessage profileUpdated)
        {
            _logger.LogInformation("Entered method UpdateProfile in ConsumerBackgroundService");

            var filter = Builders<User>.Filter.Eq(x => x.Id, profileUpdated.Id.ToString());

            var update = Builders<User>.Update
                .Set(x => x.FullName, profileUpdated.FullName)
                .Set(x => x.Phone, profileUpdated.Phone)
                .Set(x => x.Dob, profileUpdated.Dob)
                .Set(x => x.Address, profileUpdated.Address)
                .Set(x => x.Avatar, profileUpdated.Avatar)
                .Set(x => x.UpdatedBy, profileUpdated.UpdatedBy.ToString() ?? Guid.Empty.ToString())
                .Set(x => x.UpdatedAt, profileUpdated.UpdatedAt);

            var result = await _usersCollection.UpdateOneAsync(filter, update);

            _logger.LogInformation("Leaving method UpdateProfile in ConsumerBackgroundService");
        }


        private async Task CreateUserAndStaffInfo(StaffCreatedMessage staffCreated)
        {
            _logger.LogInformation("Entered method CreateUserAndStaffInfo in ConsumerBackgroundService");

            var user = new User()
            {
                Id = staffCreated.Id.ToString(),
                PasswordHash = staffCreated.PasswordHash ?? Array.Empty<byte>(),
                PasswordSalt = staffCreated.PasswordSalt ?? Array.Empty<byte>(),
                FullName = staffCreated.FullName,
                Email = staffCreated.Email,
                IsVerified = staffCreated.IsVerified,
                IsDeleted = staffCreated.IsDeleted,
                RoleId = staffCreated.RoleId,
                Status = staffCreated.Status,
                Phone = staffCreated.Phone
            };

            var staffInfo = new StaffInfo()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = user.Id,
                BranchId = staffCreated.BranchId,
                Title = staffCreated.Title
            };

            try
            {
                await _usersCollection.InsertOneAsync(user);
                await _staffInfosCollection.InsertOneAsync(staffInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateUserAndStaffInfo");
            }

            _logger.LogInformation("Leaving method CreateUserAndStaffInfo in ConsumerBackgroundService");
        }
    }
}
