using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using yogabackend.Model;

namespace yogabackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class LoginController : ControllerBase
    {


        private readonly Database _database;
        public LoginController(Database database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        [HttpPost("insert")]
        public IActionResult InsertLoginDetails([FromBody] Login login)
        {
            if (string.IsNullOrWhiteSpace(login.Username) || string.IsNullOrWhiteSpace(login.Password))
            {
                return BadRequest("Username and Password are required.");
            }

            bool success = _database.InsertLoginDetails(login.Username, login.Password);

            if (success)
            {
                return Ok("Login inserted successfully.");
            }
            else
            {
                return StatusCode(500, "Failed to insert login.");
            }
        }
    }
}
