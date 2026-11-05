using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth.API.Entities
{
    public class User : BaseEntity
    {
        public User()
        {
            Tokens = new List<Token>();
        }

        [BsonElement("passwordHash")]
        public byte[] PasswordHash { get; set; }

        [BsonElement("passwordSalt")]
        public byte[] PasswordSalt { get; set; }

        [BsonElement("fullName")]
        public string? FullName { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("isVerified")]
        public bool IsVerified { get; set; }

        [BsonRepresentation(BsonType.String)]
        [BsonElement("roleId")]
        public string? RoleId { get; set; }

        [BsonElement("status")]
        public string? Status { get; set; } 

        [BsonElement("dob")]
        public DateTime? Dob { get; set; }

        [BsonElement("address")]
        public string? Address { get; set; }

        [BsonElement("phone")]
        public string? Phone { get; set; }

        [BsonElement("avatar")]
        public string? Avatar { get; set; }

        [BsonElement("tokens")]
        public List<Token> Tokens { get; set; }
    }
}
