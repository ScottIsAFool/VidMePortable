using System;
using Newtonsoft.Json;
using PropertyChanged;
using VidMePortable.Converters;

namespace VidMePortable.Model
{
    [ImplementPropertyChanged]
    public class Auth
    {
        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("expires")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? Expires { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
}