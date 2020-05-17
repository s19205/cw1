using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public interface IStudentDbService
    {
        IEnumerable<Student> GetStudents();
        EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);
        PromoteStudentResponse PromoteStudents(PromoteStudentRequest request);
        bool IsStudentExists(String StudentIndexNumber);

        bool DeleteStudent(String StudentIndexNumber);
        bool UpdateStudent(UpdateStudentRequest request);
    }
}
