using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using Microsoft.EntityFrameworkCore;
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
        s19205Context dbContext;

        public SqlServerStudentDbService(s19205Context dbContext)
        {
            this.dbContext = dbContext;
        }

        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            try
            {
                if (IsStudentExists(request.IndexNumber) || !dbContext.Studies.Any()) { return null; }

                var study = dbContext.Studies.Where(st => st.Name == request.Studies).FirstOrDefault();
                if (study == null) { return null; }
                int studyId = study.IdStudy;

                var enrollment = dbContext.Enrollment.Where(enroll2 => 
                                            enroll2.IdStudy == studyId && enroll2.Semester == 1 && enroll2.StartDate == dbContext.Enrollment.Where(enroll => 
                                                enroll.IdStudy == studyId && enroll.Semester == 1
                                            ).Max(enroll => enroll.StartDate)).FirstOrDefault();

                if (enrollment != null)
                {
                    var tmpStudent = new Student
                    {
                        IndexNumber = request.IndexNumber,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        BirthDate = request.BirthDate,
                        IdEnrollment = enrollment.IdEnrollment
                    };
                    dbContext.Add(tmpStudent);
                    dbContext.SaveChanges();

                    var result = dbContext.Enrollment
                                .Where(el => el.IdEnrollment == enrollment.IdEnrollment)
                                .Join(dbContext.Studies,
                                      (el1) => el1.IdStudy,
                                      (el2) => el2.IdStudy,
                                      (el1, el2) => new { Semester = el1.Semester, StartDate = el1.StartDate })
                                .FirstOrDefault();

                    return new EnrollStudentResponse
                    {
                        LastName = request.LastName,
                        StartDate = result.StartDate,
                        Semester = result.Semester
                    };

                } else
                {
                    var NewEnrollment = new Enrollment
                    {
                        IdEnrollment = dbContext.Enrollment.Any() ? dbContext.Enrollment.Max(e => e.IdEnrollment) + 1 : 1,
                        IdStudy = studyId,
                        Semester = 1,
                        StartDate = DateTime.Now
                    };
                    dbContext.Add(NewEnrollment);
                    dbContext.SaveChanges();

                    var tmpStudent = new Student
                    {
                        IndexNumber = request.IndexNumber,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        BirthDate = request.BirthDate,
                        IdEnrollment = NewEnrollment.IdEnrollment
                    };

                    dbContext.Add(tmpStudent);
                    dbContext.SaveChanges();

                    var Result = dbContext.Enrollment
                               .Where(el => el.IdEnrollment == NewEnrollment.IdEnrollment)
                               .Join(dbContext.Studies,
                                     (el1) => el1.IdStudy,
                                     (el2) => el2.IdStudy,
                                     (el1, el2) => new { Semester = el1.Semester, StartDate = el1.StartDate})
                               .FirstOrDefault();

                    return new EnrollStudentResponse
                    {
                        LastName = request.LastName,
                        StartDate = Result.StartDate,
                        Semester = Result.Semester
                    };

                }
            } catch(Exception ex) { return null; }

        }

        public PromoteStudentResponse PromoteStudents(PromoteStudentRequest request)
        {
            try
            {
                if (!dbContext.Enrollment.Any() || !dbContext.Studies.Any())
                {
                    return null;
                }
                var enrollment = dbContext.Enrollment
                                          .Join(dbContext.Studies, el1 => el1.IdStudy, el2 => el2.IdStudy, (el1, el2) => new { Semester = el1.Semester, el2.Name })
                                          .Where(el => el.Semester == request.Semester && el.Name == request.Studies)
                                          .FirstOrDefault();
                if (enrollment == null)
                {
                    return null;
                }

                SqlParameter p1 = new SqlParameter("@Study", request.Studies);
                SqlParameter p2 = new SqlParameter("@Semester", request.Semester);
                
                var procedure = dbContext.Enrollment.FromSqlRaw("exec PromoteStudents @Study,@Semester", p1, p2)
                                                   .ToList()
                                                   .SingleOrDefault();
                if (procedure == null)
                {
                    return null;
                }
                return new PromoteStudentResponse
                {
                    IdEnrollment = procedure.IdEnrollment,
                    Semester = procedure.Semester,
                    StartDate = procedure.StartDate
                };

            } catch (Exception ex)
            {
                return null;
            }
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
