using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace net.Model
{
    public class User
    {
        [BsonId]
        [BsonIgnoreIfNull]
        [BsonIgnoreIfDefault]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [Required]
        [BsonElement("username")]
        public required string Username {  get; set; }

        [Required]
        [BsonElement("password")]
        public required string Password { get; set; }   

    }
}
