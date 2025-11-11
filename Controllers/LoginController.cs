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
        [HttpGet("all")]
        public IActionResult GetAllLogins()
        {
            var logins = _database.GetAllLogins();
            return Ok(logins);
        }

    }
}
