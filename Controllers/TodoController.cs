using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using net.Data;
using net.Model;
using net.Services;
using System.Text.Json;

namespace net.Controllers
{
    [ApiController]
    [Route("todos")]
    [Authorize]
    public class TodoController : Controller
    {
        private readonly MongoDBServices _db;
        private readonly ILogger<TodoController> _logger;
        private readonly IConfiguration _config;
        private readonly CloudinaryService _cloudinaryService;

        public TodoController(MongoDBServices db, ILogger<TodoController> logger, IConfiguration config, CloudinaryService cloudinaryService) {
            _db = db;
            _logger = logger;
            _config = config;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTodos()
        {
            var todos = await _db.Todos.Find(_ => true).ToListAsync();
            _logger.LogInformation("All Data fetched,Below is all the data");
            string jsonData = JsonSerializer.Serialize(todos, new JsonSerializerOptions { WriteIndented = true });
            _logger.LogInformation(jsonData);
            return Ok(new
            {
                status = "Success",
                data = todos,
                message = "Data Retrieved"
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodo(string id)
        {
            var todo = await _db.Todos.Find(t => t.Id == id).FirstOrDefaultAsync();
            string jsonData = JsonSerializer.Serialize(todo, new JsonSerializerOptions { WriteIndented = true });
            _logger.LogInformation("User fetched this ID-{id}'s data", id);
            _logger.LogInformation(jsonData);
            return todo == null ? NotFound() : Ok(new
            {
                status = "Success",
                data = todo,
                message = "Data Retrieved"
            });
        }

        [HttpGet("proxy/{filename}")]
        [AllowAnonymous]
        public async Task<IActionResult> ProxyImage(string filename)
        {
            var baseUrl = _config["CloudinarySettings:cloudinaryUrl"];
            var cloudinaryUrl = $"{baseUrl}{filename}.png";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(cloudinaryUrl);

            if (!response.IsSuccessStatusCode)
                return NotFound("Image not found");

            var contentType = response.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
            var content = await response.Content.ReadAsStreamAsync();

            return File(content, contentType);
        }

        [HttpDelete("proxy/{filename}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteImage(string filename)
        {
            var success = await _cloudinaryService.DeleteImageAsync(filename);

            if (!success)
            {
                _logger.LogWarning($"Failed to delete image: {filename}");
                return BadRequest(new
                {
                    status = "Fail",
                    message = "Image not found or could not be deleted"
                });
            }

            _logger.LogInformation($"Image deleted: {filename}");
            return Ok(new
            {
                status = "Success",
                message = "Image deleted successfully"
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo(Todo todo)
        {
            await _db.Todos.InsertOneAsync(todo);
            string jsonData = JsonSerializer.Serialize(todo, new JsonSerializerOptions { WriteIndented = true });
            _logger.LogInformation("User created this data");
            _logger.LogInformation(jsonData);
            return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
        }
        [HttpPut("{id}")]

        public async Task<IActionResult> PutTodo(string id,Todo todo)
        {
            todo.Id = id;
            var result = await _db.Todos.ReplaceOneAsync(t=>t.Id==id, todo);
            return result == null ? NotFound() : Ok(await _db.Todos.Find(t => t.Id == id).FirstOrDefaultAsync());
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchTodo(string id,Todo todo)
        {
            var update = Builders<Todo>.Update
                         .Set(t => t.Text, todo.Text)
                         .Set(t => t.completed, todo.completed);
            var result = await _db.Todos.UpdateOneAsync(t => t.Id == id, update);
            return result.MatchedCount == 0 ? NotFound() : Ok(await _db.Todos.Find(t => t.Id == id).FirstOrDefaultAsync());
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(string id)
        {
            var result = await _db.Todos.DeleteOneAsync(t => t.Id == id);
            string jsonData = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            _logger.LogInformation(jsonData);
            return result.DeletedCount == 0 ? NotFound() : Ok(new {
                status = "Success",
                ACknowledgedStatus = result.IsAcknowledged,
                DeleteCount= result.DeletedCount,
                message = "Todo Deleted Successfully"
            });
        }
    }
}
