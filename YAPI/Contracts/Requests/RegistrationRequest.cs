using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YAPI.Contracts.Requests
{
    public class RegistrationRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
