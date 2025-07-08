using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using net.Data;
using net.Helpers;
using net.Model;
namespace net.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : Controller
    {
        public readonly MongoDBServices _db;
        public readonly JwtHelpers _jwt;
        private readonly ILogger<AuthController> _logger;

        public AuthController(MongoDBServices db, JwtHelpers jwt, ILogger<AuthController> logger)
        {
            _db = db;
            _jwt = jwt;
            _logger = logger;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserDto user)
        {
            var exisitinguser = await _db.Users.Find(t => t.Username == user.Username).FirstOrDefaultAsync();
            if (exisitinguser != null) {
                _logger.LogError("User-{Username} already exists",user.Username);
                return BadRequest(new { message = "User Already Exists" });
            }
            var newuser=new User
            {
                Username= user.Username,
                Password= BCrypt.Net.BCrypt.HashPassword(user.Password)
            };

            await _db.Users.InsertOneAsync(newuser);

            _logger.LogInformation("User-{Username} is registered succesfully", user.Username);

            return Ok(new
            {
                status="SUCCESS",
                data=newuser,
                message="User Registered Succesfully"
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserDto user)
        {
            var dbuser = await _db.Users.Find(t => t.Username == user.Username).FirstOrDefaultAsync();
            
            if(dbuser == null || !BCrypt.Net.BCrypt.Verify(user.Password, dbuser.Password))
            {
                _logger.LogError("User with name-{Username} tried to enter invalid credentials", user.Username);
                return Unauthorized(new { message = "Invalid Credentials" });
            }
            var token = _jwt.TokenGenerator(dbuser);

            _logger.LogInformation("User-{Username} is logged in and token is generated,token is-{token}", user.Username,token);

            return Ok(new { token});
        }

    }
}
