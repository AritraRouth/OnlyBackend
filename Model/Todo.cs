using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace net.Model
{
    public class Todo
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        [BsonIgnoreIfNull]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [BsonElement("text")]
        public required string Text { get; set; }

        [Required]
        [BsonElement("completed")]
        public bool completed { get; set; }
    }
}
