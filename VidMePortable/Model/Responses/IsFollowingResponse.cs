using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class IsFollowingResponse : Response
    {
        [JsonProperty("isFollowing")]
        public bool IsFollowing { get; set; }
    }
}
