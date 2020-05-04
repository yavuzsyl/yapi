using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;


namespace Yapi.Contracts.External.Facebook
{
    public class FacbookUserInfoResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("picture")]
        public FacbookPicture Picture { get; set; }
    }

    public class FacbookPicture
    {
        [JsonProperty("data")]
        public FacebookPictureData Data { get; set; }
    }

    public class FacebookPictureData
    {
        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("is_silhouette")]
        public bool IsSilhouette { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("width")]
        public long Width { get; set; }
    }
}
