using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YAPI.Domain;

namespace YAPI.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(string email, string password);
        Task<AuthenticationResult> LoginAsync(string email, string password);
    }
}
