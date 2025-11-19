using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentApiBusinessLayer;
using StudentApiDataAccessLayer;
using System.Net.Http.Headers;

namespace Student_Api_Project.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/Students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllStudents")]
        public ActionResult<StudentDTO> GetAllStudents()
        {
            var students = StudentApiBusinessLayer.Student.GetAllStudents();
            return Ok(students);
        }

        [HttpGet("Passed", Name = "GetPassedStudents")]
        public ActionResult<StudentDTO> GetPassedStudents()
        {
                var passedStudents = StudentApiBusinessLayer.Student.GetPassedStudents();
            if(passedStudents.Count==0)
            {
                return NotFound("No passed students found.");
            }
                return Ok(passedStudents);
        }
        [HttpGet("Averagr",Name ="GetAverageGrade")]
        
        public ActionResult<StudentDTO> GetAverageGrade()
        {
             var averageGradeStudents = StudentApiBusinessLayer.Student.GetAverageGrad();
            if(averageGradeStudents.Count==0)
                {
                return NotFound("No students found for average grade calculation.");
            }
            return Ok(averageGradeStudents);
        }
        [HttpGet("GetByID",Name ="GetByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
         public ActionResult<StudentDTO> GetStudentByID(int id)
         {
            if(id<1)
            {
                return BadRequest("Invalid student ID.");
            }
            var student = StudentApiBusinessLayer.Student.GetStudentById(id);
            if(student==null)
            {
                return NotFound($"Student with ID {id} not found.");
            }
            return Ok(student);

        }
        [HttpPost("AddStudent",Name ="AddStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<StudentDTO> AddStudent(StudentDTO newStudent)
        {
         
            if(newStudent==null || string.IsNullOrWhiteSpace(newStudent.Name) || newStudent.Age <= 0 || newStudent.Grade < 0 || newStudent.Grade > 100)
            {
                return BadRequest("Invalid student data.");
            }
            var student= new StudentApiBusinessLayer.Student(new StudentDTO( newStudent.Id,newStudent.Name,newStudent.Age,newStudent.Grade));
            student.Save();
         newStudent.Id=student.Id;
            return CreatedAtRoute("GetByID", new { id = newStudent.Id }, newStudent);
        }
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
            if (StudentApiBusinessLayer.Student.DeletedStudent(id))
            {
                return Ok($"Student with id {id} deleted successfully.");

            }
            else
            {
                return NotFound($"Student with id {id} not found.");
            }
        }
            [HttpPut("{id}", Name = "UpdateStudent")]
                [ProducesResponseType(StatusCodes.Status200OK)]
                [ProducesResponseType(StatusCodes.Status400BadRequest)]
                [ProducesResponseType(StatusCodes.Status404NotFound)]
                public ActionResult<StudentDTO> UpdateStudent(int id, StudentDTO updatedStudent)
                {

                    if (id < 1 || updatedStudent == null || string.IsNullOrWhiteSpace(updatedStudent.Name) || updatedStudent.Age <= 0 || updatedStudent.Grade < 0 || updatedStudent.Grade > 100)
                    {
                        return BadRequest("Invalid student data.");
                    }
                    Student existingStudent = StudentApiBusinessLayer.Student.Find(id);
                    if (existingStudent == null)
                    {
                        return NotFound($"Student with ID {id} not found.");
                    }
                    existingStudent.Name = updatedStudent.Name;
                    existingStudent.Age = updatedStudent.Age;
                    existingStudent.Grade = updatedStudent.Grade;
                    if (existingStudent.Save())
                    {
                        return Ok(existingStudent);
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Error updating student.");
                    }

                }
    }
}
