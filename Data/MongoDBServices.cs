using MongoDB.Driver;
using net.Model;

namespace net.Data
{
    public class MongoDBServices
    {
        private readonly IMongoCollection<Todo> _todos;
        private readonly IMongoCollection<User> _users;

        public MongoDBServices(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionURI"]);
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _todos = database.GetCollection<Todo>(config["MongoDB:TodoCollectionName"]);
            _users=database.GetCollection<User>(config["MongoDB:UserCollectionName"]);
        }

        public IMongoCollection<Todo> Todos => _todos;
        public IMongoCollection<User> Users=> _users;  
    }
}
