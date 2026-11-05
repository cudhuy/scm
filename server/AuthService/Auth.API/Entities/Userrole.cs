using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth.API.Entities
{
    public class UserRole : BaseEntity
    {
        [BsonElement("roleName")]
        public string? RoleName { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }
    }
}
