using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class UserResponse : FollowResponse
    {
        [JsonProperty("user")]
        public User User { get; set; }
    }
}
