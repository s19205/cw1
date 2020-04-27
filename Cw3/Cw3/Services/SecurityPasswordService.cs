using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Cw3.Services
{
    public class SecurityPasswordService : IPasswordService
    {
        public string CreateValue()
        {
            byte[] randomBytes = new byte[16];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }

        public string HashPassword(string password, string value)
        {
            var valueBytes = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: Encoding.UTF8.GetBytes(value),
                    prf: KeyDerivationPrf.HMACSHA512,
                    iterationCount: 10000,
                    numBytesRequested: 256 / 8
                );
            return Convert.ToBase64String(valueBytes);
        }

        public bool ValidatePassword(string hash, string password, string value)
        {
            return HashPassword(password, value) == hash;
        }
    }
}
