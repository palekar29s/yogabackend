using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using yogabackend.Model;

namespace yogabackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {


        private readonly Database _database;
        public CommentController(Database database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }


      /// <summary>
      /// /
      /// </summary>
      /// <param //name="comment"></param>
      /// <returns></returns>
        [HttpPost("add")]
        public IActionResult AddComment([FromBody] Contactus comment)
        {
            if (comment == null)
                return BadRequest("Comment data is required.");

            // Assuming StudentId is taken from logged-in user, or provided in comment
            bool result = _database.AddComment(comment);

            if (result)
                return Ok(new { message = "Comment added successfully." });
            else
                return StatusCode(500, "Failed to add comment.");
        }
        [HttpGet("all")]
        public IActionResult GetAllBookings()
        {
            var bookings = _database.GetAllBookings(); // returns List<Booking>
            return Ok(bookings);
        }


    }
}
