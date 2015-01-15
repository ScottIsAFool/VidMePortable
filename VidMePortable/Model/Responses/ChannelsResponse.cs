using System.Collections.Generic;
using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class ChannelsResponse : Response
    {
        [JsonProperty("channels")]
        public List<Channel> Channels { get; set; }
    }

}
