using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yapi.Web.Models
{
    public partial class Temperatures
    {
        [JsonProperty("authResponse")]
        public AuthResponse authResponse { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }
    }

    public partial class AuthResponse
    {
        [JsonProperty("accessToken")]
        public string accessToken { get; set; }

        [JsonProperty("userID")]
        public string userId { get; set; }

        [JsonProperty("expiresIn")]
        public long expiresIn { get; set; }

        [JsonProperty("signedRequest")]
        public string signedRequest { get; set; }

        [JsonProperty("graphDomain")]
        public string graphDomain { get; set; }

        [JsonProperty("data_access_expiration_time")]
        public long dataAccessExpirationTime { get; set; }
    }
}
