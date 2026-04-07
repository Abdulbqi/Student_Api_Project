using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Student_Api_Project.Authorization;
using Student_Api_Project.DataSimulation;
using StudentApiBusinessLayer;
using StudentApiDataAccessLayer;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Student_Api_Project.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet("All", Name = "GetAllStudents")]
        public ActionResult<StudentDTO> GetAllStudents()
        {
            var students = DataSimulation.StudentDataSimulation.Students;
            return Ok(students);
        }
        [AllowAnonymous]
        [HttpGet("Passed", Name = "GetPassedStudents")]
        public ActionResult<Model.Student> GetPassedStudents()
        {
           
            var passedStudents = DataSimulation.StudentDataSimulation.Students.Where(s => s.Grade >= 50).ToList();

            if (passedStudents.Count==0)
            {
                return NotFound("No passed students found.");
            }
                return Ok(passedStudents);
        }
        [AllowAnonymous]
        [HttpGet("Averagr",Name ="GetAverageGrade")]
        
        public ActionResult<Model.Student> GetAverageGrade()
        {
            if (StudentDataSimulation.Students.Count==0)
                {
                return NotFound("No students found for average grade calculation.");
            }
            var averageGradeStudents = DataSimulation.StudentDataSimulation.Students.Average(student => student.Grade);
            return Ok(averageGradeStudents);
        }
        [HttpGet("GetByID",Name ="GetByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Student>> GetStudentById( int id,
                     [FromServices] IAuthorizationService authorizationService)
        {
            if (id < 1)
                return BadRequest("Invalid student id.");

            var student = StudentDataSimulation.Students
                .FirstOrDefault(s => s.Id == id);

            if (student == null)
                return NotFound("Student not found.");

            var authResult = await authorizationService.AuthorizeAsync(
                User,
                id,
                "StudentOwnerOrAdmin");

            if (!authResult.Succeeded)
                return Forbid(); 

            return Ok(student);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("AddStudent",Name ="AddStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<StudentDTO> AddStudent(Model.Student newStudent)
        {
         
            if(newStudent==null || string.IsNullOrWhiteSpace(newStudent.Name) || newStudent.Age <= 0 || newStudent.Grade < 0 || newStudent.Grade > 100)
            {
                return BadRequest("Invalid student data.");
            }
            newStudent.Id = StudentDataSimulation.Students.Count > 0 ? StudentDataSimulation.Students.Max(s => s.Id) + 1 : 1;
            StudentDataSimulation.Students.Add(newStudent);
            return CreatedAtRoute("GetByID", new { id = newStudent.Id }, newStudent);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteStudent(int id)
        {
            if (id < 1)
            {
                return BadRequest("");
            }
            var student = StudentDataSimulation.Students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound($"Student with ID {id} not found.");
            }

            StudentDataSimulation.Students.Remove(student);
            return Ok($"Student with ID {id} has been deleted.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
                public ActionResult<Model.Student> UpdateStudent(int id, Model.Student updatedStudent)
                {

                 if (id < 1 || updatedStudent == null || string.IsNullOrEmpty(updatedStudent.Name) || updatedStudent.Age < 0 || updatedStudent.Grade < 0)
                 {
                     return BadRequest("Invalid student data.");
                 }

                  var student = StudentDataSimulation.Students.FirstOrDefault(s => s.Id == id);
                 if (student == null)
                 {
                    return NotFound($"Student with ID {id} not found.");
                 }

                    student.Name = updatedStudent.Name;
                    student.Age = updatedStudent.Age;
                    student.Grade = updatedStudent.Grade;

                   return Ok(student);


                }
        
    }
}
