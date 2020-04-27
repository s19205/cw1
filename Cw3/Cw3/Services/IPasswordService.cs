using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cw3.Services
{
     public interface IPasswordService
    {
        String HashPassword(String password, String value);
        String CreateValue();
        bool ValidatePassword(String hash, String password, String value);
    }
}
