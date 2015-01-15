using Newtonsoft.Json;
using PropertyChanged;

namespace VidMePortable.Model.Responses
{
    [ImplementPropertyChanged]
    public class VideoInfoResponse : Response
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }
    }
}
