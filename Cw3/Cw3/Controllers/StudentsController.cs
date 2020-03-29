using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        private const string ConString = "Data Source=db-mssql;Initial Catalog=s19205;Integrated Security=True";
        public StudentsController(IDbService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult GetStudents()                  //4.2
        {
            var list = new List<Student>();
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select FirstName, LastName, BirthDate, Name, Semester from student INNER JOIN Enrollment ON student.IdEnrollment = Enrollment.IdEnrollment INNER JOIN Studies ON Enrollment.IdStudy = Studies.IdStudy";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student();
                    //st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.Name = dr["Name"].ToString();
                    st.Semester = dr.GetInt32(4);
                   // st.IdEnrollment = dr.GetInt32(4);

                    list.Add(st);
                }

            }
            
            return Ok(list);
        }

        [HttpGet("{indexNumber}")]
        public IActionResult GetStudent(string indexNumber)     //4.3
        {
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText = "select FirstName, LastName, BirthDate, Name, Semester from student INNER JOIN Enrollment ON student.IdEnrollment = Enrollment.IdEnrollment INNER JOIN Studies ON Enrollment.IdStudy = Studies.IdStudy where indexnumber = @index";

                /*
                SqlParameter par = new SqlParameter();
                par.Value = indexNumber;
                par.ParameterName = "index";
                com.Parameters.Add(par);
                */

                com.Parameters.AddWithValue("index", indexNumber);

                con.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student();

                    /*
                    if (dr["IndexNumber"] == DBNull.Value)
                    {
                    }
                    */

                    //st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                    st.BirthDate = dr["BirthDate"].ToString();
                    st.Name = dr["Name"].ToString();
                    st.Semester = dr.GetInt32(4);
                    // st.IdEnrollment = dr.GetInt32(4);
                    return Ok(st);
                }

            }
                return NotFound();
        }

        
                /*   [HttpGet("{id}")]
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
                   */
            }
}