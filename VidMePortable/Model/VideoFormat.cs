using Newtonsoft.Json;
using PropertyChanged;

namespace VidMePortable.Model
{
    [ImplementPropertyChanged]
    public class VideoFormat
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }
}