using System.Collections.Generic;
using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class ChannelsResponse : Response
    {
        [JsonProperty("data")]
        public List<Channel> Channels { get; set; }
    }
}
