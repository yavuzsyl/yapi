using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YAPI.Contracts.Responses
{
    public class RegistrationResponse
    {
        public string Token { get; set; }
        public IEnumerable<string> Messages { get; set; }
        public bool Success { get; set; }
    }
}
