using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentDbService _service;
      
        public EnrollmentsController(IStudentDbService service)
        {
            _service = service;
        }



        [HttpPost]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            return StatusCode(201, _service.EnrollStudent(request));
        }

        [HttpPost("promotions")]
        public IActionResult PromoteStudents(PromoteStudentRequest request)
        {
            return StatusCode(201, _service.PromoteStudents(request));
        }

    

    }
}