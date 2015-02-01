using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class ChannelResponse : FollowResponse
    {
        [JsonProperty("channel")]
        public Channel Channel { get; set; }
    }
}