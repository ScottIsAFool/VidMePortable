using System.Collections.Generic;
using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class VideosResponse : Response
    {
        [JsonProperty("page")]
        public Page Page { get; set; }

        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; }

        [JsonProperty("videos")]
        public List<Video> Videos { get; set; }

        //[JsonProperty("watching")]
        //public object[] Watching { get; set; }

        [JsonProperty("viewerVotes")]
        public object[] ViewerVotes { get; set; }
    }
}
