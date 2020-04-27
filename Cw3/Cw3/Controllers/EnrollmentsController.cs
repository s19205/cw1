using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Cw3.DTOs.Requests;
using Cw3.DTOs.Responses;
using Cw3.Models;
using Cw3.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Cw3.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        IStudentDbService _service;
        IPasswordService passwordService;
        IConfiguration configuration;

        public EnrollmentsController(IStudentDbService service, IPasswordService passwordService, IConfiguration configuration)
        {
            this._service = service;
            this.passwordService = passwordService;
            this.configuration = configuration;
        }

        [HttpPost]
        [Authorize]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            return StatusCode(201, _service.EnrollStudent(request));
        }

        [HttpPost("promotions")]
        [Authorize]
        public IActionResult PromoteStudents(PromoteStudentRequest request)
        {
            return StatusCode(201, _service.PromoteStudents(request));
        }

        [AllowAnonymous]
        [HttpPost("refresh-token/{refreshToken}")]
        public IActionResult refreshToken(String refreshToken)
        {
            string login = _service.GetRefreshTokenOwner(refreshToken);
            if (login == null)
            {
                return BadRequest("Wrong refresh token");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name,login),
                new Claim(ClaimTypes.Role,"Student")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "Gakko",
                audience: "Student",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });

        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            if (!_service.IsStudentExists(request.Login))
            {
                return BadRequest("WRONG PASSWORD OR LOGIN");
            }

            var requestedPasswordsData = _service.getStudentPasswordData(request.Login);
            if (!passwordService.ValidatePassword(requestedPasswordsData.Password, request.Password, requestedPasswordsData.Value))
            {
                return BadRequest("Wrong password or login");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name,request.Login),
                new Claim(ClaimTypes.Role,"Student")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "Gakko",
                audience: "Student",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );
            var TmpRefreshToken = Guid.NewGuid();
            _service.SetRefreshToken(request.Login, TmpRefreshToken.ToString());
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refershToken = TmpRefreshToken
            });
        }

    }
}