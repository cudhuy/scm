using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth.API.Entities
{
    public class StaffInfo : BaseEntity
    {
        [BsonRepresentation(BsonType.String)]
        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("branchId")]
        public string BranchId { get; set; }
    }
}
