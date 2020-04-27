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

  /*      [HttpGet]
        public IActionResult GetStudents()
        {
            var list = new List<Student>();
            list.Add(new Student { IndexNumber = "1", FirstName = "Jan", LastName = "Kowalski" });
            list.Add(new Student { IndexNumber = "2", FirstName = "Andrzej", LastName = "Malewski" });

            return Ok(list);
        }

        [HttpGet("{id}")]

        public IActionResult GetStudent(int id)
        {

            return Ok(new Student { IndexNumber = "1", FirstName = "Andrzej", LastName = "Malewski" });
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
                //    st.BirthDate = dr["BirthDate"].ToString();
                //    st.Name = dr["Name"].ToString();
                //    st.Semester = dr.GetInt32(4);
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

                com.Parameters.AddWithValue("index", indexNumber);

                con.Open();
                var dr = com.ExecuteReader();
                if (dr.Read())
                {
                    var st = new Student();

                    //st.IndexNumber = dr["IndexNumber"].ToString();
                    st.FirstName = dr["FirstName"].ToString();
                    st.LastName = dr["LastName"].ToString();
                 //   st.BirthDate = dr["BirthDate"].ToString();
                 //   st.Name = dr["Name"].ToString();
                 //   st.Semester = dr.GetInt32(4);
                    // st.IdEnrollment = dr.GetInt32(4);
                    return Ok(st);
                }

            }
                return NotFound();
        }
        */

    }
}