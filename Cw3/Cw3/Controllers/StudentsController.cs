using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw3.DAL;
using Cw3.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        List<Student> sList = new List<Student>();
        private readonly IDbService _dbService;
        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudent(string orderBy)
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(int id)
        {
            if(id == 1)
            {
                return Ok("Kowalski");
            } else if(id == 2)
            {
                return Ok("Malewski");
            }

            return NotFound("Nie znaleziono studenta");
        }

        [HttpPost]
        public IActionResult CreateStudent(Student student)
        {
            student.IndexNumber = $"s{new Random().Next(1, 20000)}";
            sList.Add(new Student() {IdStudent=student.IdStudent,FirstName=student.FirstName,LastName=student.LastName,IndexNumber=student.IndexNumber });
            return Ok(student);
        }

        [HttpPut("{id}")]
        public IActionResult PutStudent(int id, Student student)
        {
            if (sList.Exists(x => x.IdStudent == id))
            {
                var st = sList.FirstOrDefault(s => s.IdStudent == id);
                if (st != null)
                {
                    st.FirstName = student.FirstName;
                    st.LastName = student.LastName;
                    st.IndexNumber = student.IndexNumber;
                    return Ok("Aktualizacja dokonczona");
                }
                else
                    return NotFound("Nie znaleziono studenta");              
            }
            return NotFound("Nie znaleziono studenta");

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            if (sList.Exists(x => x.IdStudent == id))
            {
                sList.RemoveAt(id);
                return Ok("Usuwanie ukonczone");
            }
            return NotFound("Nie znaleziono studenta");
        }
    }
}