using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Auth.API.Constants;

namespace Auth.API.Entities
{
    public abstract class BaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [BsonElement("createdAt")] // Ánh xạ với createdAt trong MongoDB
        public DateTime CreatedAt { get; set; }

        [BsonElement("createdBy")]
        public string CreatedBy { get; set; }

        [BsonElement("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("updatedBy")]
        public string UpdatedBy { get; set; }

        [BsonElement("isDeleted")]
        [BsonRepresentation(BsonType.Int32)] // Chuyển bool thành int trong MongoDB
        public bool IsDeleted { get; set; }

        public BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsDeleted = false;
            CreatedBy = AppConstants.SYSTEM_ROLE_ID.ToString();
            UpdatedBy = AppConstants.SYSTEM_ROLE_ID.ToString();
        }
    }
}
