using Microsoft.AspNetCore.Mvc;
using yogabackend.Model;

namespace yogabackend.Controllers
{
    public class imageController : Controller
    {

        private readonly Database _database;
        public imageController(Database database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        // GET all wandering images
        [HttpGet("alll")]
        public IActionResult GetAllWanderingImages()
        {
            var images = _database.GetAllWanderingImages();
            return Ok(images);
        }

        [HttpPost("insert")]
        public IActionResult InsertWanderingImage([FromBody] Wanderimage model)
        {
            if (model == null || model.StudentId <= 0)
                return BadRequest("Invalid wandering image data.");

            bool result = _database.InsertWanderingImage(model.StudentId, model.Description, model.LostThingImage);

            if (result)
                return Ok(new { message = "Wandering image inserted successfully." });
            else
                return StatusCode(500, "Failed to insert wandering image.");
        }
        [HttpPut("update/{id}")]
        public IActionResult UpdateWanderingImage(int id, [FromBody] Wanderimage model)
        {
            if (model == null)
                return BadRequest("Invalid data.");

            bool result = _database.UpdateWanderingImage(id, model.Description, model.LostThingImage);

            if (result)
                return Ok(new { message = "Wandering image updated successfully." });
            else
                return NotFound("Wandering image not found.");
        }
        [HttpDelete("delete/{id}")]
        public IActionResult DeleteWanderingImage(int id)
        {
            bool result = _database.DeleteWanderingImage(id);

            if (result)
                return Ok(new { message = "Wandering image deleted successfully." });
            else
                return NotFound("Wandering image not found.");
        }

    }
}
