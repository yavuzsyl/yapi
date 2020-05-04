using System;
using System.Collections.Generic;
using System.Text;

namespace Yapi.Contracts.V1.Responses
{
    public class AuthenticationResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Success { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
