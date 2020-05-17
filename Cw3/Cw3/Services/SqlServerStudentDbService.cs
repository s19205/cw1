using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{

    public class SqlServerStudentDbService : IStudentDbService
    {
        private int semester;
        private DateTime startDate;
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s19205;Integrated Security=True";

        s19205Context dbContext;

        public SqlServerStudentDbService(s19205Context dbContext)
        {
            this.dbContext = dbContext;
        }

        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            using (SqlConnection con = new SqlConnection(ConString))
            {
                con.Open();
                var tran = con.BeginTransaction();

                int idStudies = 0;
                int idEnrollment = 0;

                using (SqlCommand com1 = new SqlCommand())
                {
                    com1.Connection = con;
                    try
                    {
                        com1.Transaction = tran;
                        //1. czy istnieja studia?
                        com1.CommandText = "select IdStudy from Studies  where Name = @name";
                        com1.Parameters.AddWithValue("name", request.Studies);
                        var dr = com1.ExecuteReader();

                        if (!dr.Read())
                        {
                            dr.Close();
                            tran.Rollback();
                            //return BadRequest("Studia nie istnieja!");
                        }
                        idStudies = (int)dr["IdStudy"];
                        dr.Close();
                    }
                    catch (SqlException exc)      //kiedy bled ktorego nie przewidzialismy
                    {
                        tran.Rollback();
                    }
                }
                using (SqlCommand com2 = new SqlCommand())
                {
                    com2.Connection = con;
                    try
                    {
                        com2.Transaction = tran;
                        com2.CommandText = "select max(idenrollment)+1 from enrollment";
                        int idEnr;
                        var dr = com2.ExecuteReader();
                        if (dr.Read())
                        {
                            idEnr = dr.GetInt32(0);
                        }
                        dr.Close();
                    }
                    catch (SqlException exc)      //kiedy bled ktorego nie przewidzialismy
                    {
                        tran.Rollback();
                    }
                }
                if (idStudies > 0)
                {
                    using (SqlCommand com3 = new SqlCommand())
                    {
                        com3.Connection = con;
                        try
                        {
                            //3. szukamy czy istnije taki semestr
                            com3.Transaction = tran;
                            com3.CommandText = "SELECT IdEnrollment FROM Enrollment WHERE Semester=1 AND IdStudy=@idStudies";
                            com3.Parameters.AddWithValue("idStudies", idStudies);
                            var dr = com3.ExecuteReader();
                            if (!dr.Read()) //jesli nie istnije to dodajemy do bazy danych
                            {

                                com3.CommandText = "insert into enrollments values(Max(IdEnrollment+1), 1, @idStudy, CONVERT(date, GETDATE()))";
                                com3.Parameters.AddWithValue("idStudy", idStudies);
                            }
                            idEnrollment = (int)dr["IdEnrollment"];
                            dr.Close();
                        }
                        catch (SqlException exc)      //kiedy bled ktorego nie przewidzialismy
                        {
                            tran.Rollback();
                        }
                    }
                }
                using (SqlCommand com4 = new SqlCommand())
                {
                    com4.Connection = con;
                    try
                    {

                        //4. zapisanie semestru i date na pryszlosc
                        com4.Transaction = tran;
                        com4.CommandText = "select semester, startdate from enrollment where IdEnrollment=@idEnrollment";
                        com4.Parameters.AddWithValue("idEnrollment", idEnrollment);
                        var dr = com4.ExecuteReader();
                        if (dr.Read())
                        {
                            semester = (int)dr["semester"];
                            startDate = (DateTime)dr["startdate"];
                        }
                        dr.Close();
                    }
                    catch (SqlException exc)      //kiedy bled ktorego nie przewidzialismy
                    {
                        tran.Rollback();
                    }
                }
                using (SqlCommand com5 = new SqlCommand())
                {
                    com5.Connection = con;
                    try
                    {
                        //5. czy podany indeks studenta jest unikalny
                        com5.Transaction = tran;
                        com5.CommandText = "select * from Student where indexnumber = @index";
                        com5.Parameters.AddWithValue("index", request.IndexNumber);
                        var dr = com5.ExecuteReader();
                        if (dr.Read())
                        {
                            dr.Close();
                            tran.Rollback();
                           // return BadRequest("Student uz istnieje!");
                        }
                        dr.Close();
                    }
                    catch (SqlException exc)      //kiedy bled ktorego nie przewidzialismy
                    {
                        tran.Rollback();
                    }
                }
                using (SqlCommand com6 = new SqlCommand())
                {
                    com6.Connection = con;
                    try
                    {
                        //6. dodanie studenta
                        com6.Transaction = tran;
                        com6.CommandText = "insert into Student values (@index, @Fname, @Lname, @birthday, @idEnrollment)";
                        com6.Parameters.AddWithValue("index", request.IndexNumber);
                        com6.Parameters.AddWithValue("Fname", request.FirstName);
                        com6.Parameters.AddWithValue("Lname", request.LastName);
                        com6.Parameters.AddWithValue("birthday", request.BirthDate);
                        com6.Parameters.AddWithValue("idEnrollment", idEnrollment);

                        com6.ExecuteNonQuery();

                        //kiedy wszystko ok
                        tran.Commit();
                    }
                    catch (SqlException exc)      //kiedy bled ktorego nie przewidzialismy
                    {
                        tran.Rollback();
                    }
                }
            }
            var response = new EnrollStudentResponse();
            response.LastName = request.LastName;
            response.Semester = semester;
            response.StartDate = startDate;

            return response;
        }

        public PromoteStudentResponse PromoteStudents(PromoteStudentRequest request)
        {
            var response = new PromoteStudentResponse();
            using (SqlConnection con = new SqlConnection(ConString))
            {
                con.Open();
                SqlCommand com = new SqlCommand("promoteStudent", con);
                com.CommandType = CommandType.StoredProcedure;

                com.Parameters.Add(new SqlParameter("@Studies", request.Studies));
                com.Parameters.Add(new SqlParameter("@Semester", request.Semester));

                using (SqlDataReader rdr = com.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        response.IdEnrollment = (int)rdr["IdEnrollment"];
                        response.Semester = (int)rdr["Semester"];
                        response.StartDate = (DateTime)rdr["StartDate"];
                    }
                    rdr.Close();
                }
                con.Close();
            }
            return response;
        }

        public bool IsStudentExists(string StudentIndexNumber)
        {
            return dbContext.Student.Any() ? dbContext.Student.Where(student => student.IndexNumber == StudentIndexNumber).FirstOrDefault() == null : false;
        }

        public IEnumerable<Student> GetStudents()
        {
            return dbContext.Student.ToList();
        }

        public bool DeleteStudent(string StudentIndexNumber)
        {
            try
            {
                var student = dbContext.Student.Where(stud => stud.IndexNumber == StudentIndexNumber).FirstOrDefault();
                if (student == null)
                {
                    return false;
                }
                dbContext.Student.Remove(student);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateStudent(UpdateStudentRequest request)
        {
            try
            {
                var student = dbContext.Student.Where(student => student.IndexNumber == request.IndexNumber).FirstOrDefault();
                if (student == null)
                {
                    return false;
                }

                student.FirstName = request.FirstName;
                student.LastName = request.LastName;
                student.IdEnrollment = request.IdEnrollment;
                student.BirthDate = request.BirthDate;
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
