using System;
using Newtonsoft.Json;

namespace VidMePortable.Model
{
    public class Auth
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("expires")]
        public DateTime Expires { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}