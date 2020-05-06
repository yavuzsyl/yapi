using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Yapi.Contracts.External.Facebook;
using YAPI.Options;

namespace YAPI.Services
{
    public class FacebookAuthService : IFacebookAuthService
    {
        private const string TokenValidationUrl = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}|{2}";
        private const string UserInfoUrl = "https://graph.facebook.com/v6.0/me?fields=id,name,last_name,first_name,email,picture&access_token={0}";
        private readonly FacebookAuthSettings facebookAuthSettings;
        private readonly IHttpClientFactory httpClientFactory;

        public FacebookAuthService(IHttpClientFactory httpClientFactory, FacebookAuthSettings facebookAuthSettings)
        {
            this.httpClientFactory = httpClientFactory;
            this.facebookAuthSettings = facebookAuthSettings;
        }

        public async Task<FacbookUserInfoResult> GetUserInfoAsync(string accessToken)
        {
            var formattedUrl = string.Format(UserInfoUrl, accessToken);

            var result = await httpClientFactory.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();

            var responseAsString = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FacbookUserInfoResult>(responseAsString);
        }

        public async Task<FacbookTokenValidationResult> VlidateAccessTokenAsync(string accessToken)
        {
            try
            {
                var formattedUrl = string.Format(TokenValidationUrl, accessToken, facebookAuthSettings.AppId, facebookAuthSettings.AppSecret);

                var result = await httpClientFactory.CreateClient().GetAsync(formattedUrl);
                result.EnsureSuccessStatusCode();

                var responseAsString = await result.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<FacbookTokenValidationResult>(responseAsString);
            }
            catch (Exception)
            {
                return new FacbookTokenValidationResult();
            }




        }
    }
}
