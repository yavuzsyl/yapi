using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yapi.Contracts.External.Facebook;

namespace YAPI.Services
{
    public interface IFacebookAuthService
    {
        Task<FacbookTokenValidationResult> VlidateAccessTokenAsync(string accessToken);
        Task<FacbookUserInfoResult> GetUserInfoAsync(string accessToken);
    }
}
