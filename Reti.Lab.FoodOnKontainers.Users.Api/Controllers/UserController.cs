using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Reti.Lab.FoodOnKontainers.Middleware;

namespace Reti.Lab.FoodOnKontainers.Users.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly User.IUserService _userService;
        private readonly ILogService _logger;

        public UserController(ILogService logger, User.IUserService userSvc)
        {
            _logger = logger;
            _userService = userSvc;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate(string username, string password)
        {            
            var user = _userService.Authenticate(username, password);

            if (user == null)
            {
                _logger.Log("Username or password is incorrect", LogLevel.Error, System.Net.HttpStatusCode.BadRequest, "UserService");
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            _logger.Log("Auth avvenuta con successo", LogLevel.Information, System.Net.HttpStatusCode.OK, "UserService");

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]Dto.User userParam)
        {
            var result = _userService.Register(userParam);

            if (result == false)
                return BadRequest();

            return Ok();
        }

        [HttpGet("getUser/{id}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            var user = await _userService.GetUser(userId);

            return Ok(user);
        }

        [HttpPut("updateUser/{id}")]
        public IActionResult UpdateUser([FromBody]Dto.User userParam)
        {
            var result = _userService.UpdateUser(userParam);

            if (result == false)
                return BadRequest();

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("getAll")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            _logger.Log("Users recuperati con successo", LogLevel.Information, System.Net.HttpStatusCode.OK, "UserService");
            return Ok(users);
        }

        [HttpGet("addFavorite/{userId}/{restaurantId}")]
        public IActionResult AddFavorite(int userId, int restaurantId)
        {
            var result = _userService.AddFavorite(userId, restaurantId);

            if (result == false)
                return BadRequest();

            return Ok();
        }

        [HttpGet("removeFavorite/{userId}/{restaurantId}")]
        public IActionResult RemoveFavorite(int userId, int restaurantId)
        {
            var result = _userService.RemoveFavorite(userId, restaurantId);

            if (result == false)
                return BadRequest();

            return Ok();
        }
    }  
}