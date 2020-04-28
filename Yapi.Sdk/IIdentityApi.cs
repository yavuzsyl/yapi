using Refit;
using System;
using System.Threading.Tasks;
using Yapi.Contracts.V1.Requests;
using Yapi.Contracts.V1.Responses;

namespace Yapi.Sdk
{
    public interface IIdentityApi
    {
        [Post("/api/v1/identity/register")]
        Task<ApiResponse<AuthenticationResponse>> RegisterAsync([Body] RegistrationRequest registrationRequest);

        [Post("/api/v1/identity/login")]
        Task<ApiResponse<AuthenticationResponse>> LoginAsync([Body] LoginRequest loginRequest);

        [Post("/api/v1/identity/refersh")]
        Task<ApiResponse<AuthenticationResponse>> RefreshAsync([Body] RefreshTokenRequest refreshRequest);
    }
}
