using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using yogabackend.Model;

namespace yogabackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class studentController : ControllerBase
    {
        private readonly Database _database;
        public studentController(Database database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }
        public List<string> fruit = new List<string> { "apple", "banana", "mango" };
        [HttpGet]
        public List<string> GetFruit()
        {
            return fruit;
        }

        [HttpGet("all")]
        public IActionResult GetAllStudents()
        {
            var students = _database.GetAllStudents();
            return Ok(students);
        }

        [HttpPut("update/{id}")]
        public IActionResult UpdateStudent(int id, [FromBody] Student student)
        {
            if (student == null)
            {
                return BadRequest("Invalid student data.");
            }

          

            // Update Student details
            var updated = _database.UpdateStudent(id, student);
            if (updated)
                return Ok(new { message = "Student updated successfully" });
            else
                return NotFound(new { message = "Student not found or update failed" });
        }

        [HttpPost("insert")]
        public IActionResult InsertStudent([FromBody] CreateStudentRequest student)
        {
            if (student == null)
                return BadRequest("Invalid student data.");

            bool result = _database.InsertStudent(student);

            if (result)
                return Ok(new { message = "Student inserted successfully." });
            else
                return StatusCode(500, "Failed to insert student.");
        }
        [HttpGet("bylogin/{loginId}")]
        public IActionResult GetStudentByLoginId(int loginId)
        {
            var student = _database.GetStudentByLoginId(loginId);
            if (student == null)
            {
                return NotFound();
            }
            return Ok(student);
        }

        
    }
}
