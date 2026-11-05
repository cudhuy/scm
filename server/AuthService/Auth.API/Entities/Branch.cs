using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth.API.Entities
{
    public class Branch : BaseEntity
    {
        [BsonRequired]
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonRequired]
        [BsonElement("address")]
        public string Address { get; set; }

        [BsonRequired]
        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("type")]
        public string Type { get; set; } // "Main" hoặc "Sub"

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;

        [BsonElement("staffs")]
        public List<StaffInfo> Staffs { get; set; } = new List<StaffInfo>();
    }
}
