using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class ChannelResponse : Response
    {
        [JsonProperty("channel")]
        public Channel Channel { get; set; }
    }
}