using Newtonsoft.Json;

namespace VidMePortable.Model
{
    public class Page
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("offset")]
        public int Offset { get; set; }

        [JsonProperty("limit")]
        public int Limit { get; set; }
    }
}