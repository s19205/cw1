using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.DTOs.Requests
{
    public class EnrollStudentRequest
    {
        //[Range(1,10)] od 1 do 10
        [Required(ErrorMessage = "Musisz podac indeks studenta")]
        [RegularExpression("^s[0-9]+$")]
        public string IndexNumber { get; set; }
       
        [Required(ErrorMessage = "Musisz podac imie studenta")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Musisz podac nazwisko studenta")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Musisz podac date urodzenia studenta")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Musisz podac prawdziwe studia")]
        public string Studies { get; set; }
    }
}
