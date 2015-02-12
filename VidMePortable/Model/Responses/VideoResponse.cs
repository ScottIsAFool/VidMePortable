using System.Collections.Generic;
using Newtonsoft.Json;
using PropertyChanged;

namespace VidMePortable.Model.Responses
{
    [ImplementPropertyChanged]
    public class VideoProgress
    {
        [JsonProperty("progress")]
        public int Progress { get; set; }
    }

    [ImplementPropertyChanged]
    public class Watchers
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("countries")]
        public List<string> Countries { get; set; }
    }

    public class VideoResponse : Response
    {
        [JsonProperty("video")]
        public Video Video { get; set; }

        [JsonProperty("progress")]
        public VideoProgress Progress { get; set; }

        [JsonProperty("watchers")]
        public Watchers Watchers { get; set; }

        [JsonProperty("viewerVote")]
        public ViewerVote ViewerVote { get; set; }

        [JsonProperty("isFeatured")]
        public bool IsFeatured { get; set; }
    }
}
