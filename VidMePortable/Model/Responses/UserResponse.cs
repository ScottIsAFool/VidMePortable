using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class UserResponse : Response
    {
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
