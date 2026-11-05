using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth.API.Entities
{
    public class Token : BaseEntity
    {
        [BsonRepresentation(BsonType.String)]
        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("tokenValue")]
        public string TokenValue { get; set; }

        [BsonElement("tokenType")]
        public string TokenType { get; set; }

        [BsonElement("expirationDate")]
        public DateTime? ExpirationDate { get; set; }

        [BsonElement("isRevoked")]
        public bool? IsRevoked { get; set; }
    }
}
