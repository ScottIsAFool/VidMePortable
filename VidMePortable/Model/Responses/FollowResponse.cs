using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class FollowResponse : Response
    {
        [JsonProperty("isFollowing")]
        public bool IsFollowing { get; set; }

        [JsonProperty("isFollowedBy")]
        public bool IsFollowedBy { get; set; }
    }
}
