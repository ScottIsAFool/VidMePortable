using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class VideoUploadResponse
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("duration")]
        public double Duration { get; set; }

        [JsonProperty("video")]
        public Video Video { get; set; }
    }

}
