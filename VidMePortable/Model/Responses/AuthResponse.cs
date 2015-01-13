using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class AuthResponse : Response
    {
        [JsonProperty("auth")]
        public Auth Auth { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }

}
