using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yapi.Contracts.V1.Responses;

namespace YAPI.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResponse> RegisterAsync(string email, string password);
        Task<AuthenticationResponse> LoginAsync(string email, string password);
        Task<AuthenticationResponse> RefreshTokenAsync(string token, string refreshToken);
        //facebook api den gelen tokenı basacağız kardeş
        Task<AuthenticationResponse> LoginWithFacebookAsync(string facebookAccessToken);

    }
}
