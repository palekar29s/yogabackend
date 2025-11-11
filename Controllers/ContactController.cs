using Microsoft.AspNetCore.Mvc;
using yogabackend.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace yogabackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly Database _database;
        public ContactController(Database database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        [HttpGet("get/{id}")]
        public IActionResult GetContactById(int id)
        {
            var contact = _database.GetContactById(id);
            if (contact == null)
                return NotFound();

            return Ok(contact);
        }
        [HttpPost("add")]
        public IActionResult AddContact([FromBody] Contact contact)
        {
            if (contact == null)
                return BadRequest();

            bool isAdded = _database.AddContact(contact);
            if (isAdded)
                return Ok(new { message = "Contact added successfully!" });

            return StatusCode(500, "Error adding contact.");
        }

        // DELETE contact by ID
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteContact(int id)
        {
            bool isDeleted = _database.DeleteContact(id);
            if (isDeleted)
                return Ok(new { message = "Contact deleted successfully!" });

            return NotFound();
        }
    }
}
