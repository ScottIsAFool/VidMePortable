using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class VideoRequestResponse : Response
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("video")]
        public Video Video { get; set; }

        [JsonProperty("maxSize")]
        public int? MaxSize { get; set; }

        [JsonProperty("maxSizeMB")]
        public int? MaxSizeMB { get; set; }

        [JsonProperty("uploadId")]
        public int? UploadId { get; set; }

        [JsonProperty("accessToken")]
        public Auth AccessToken { get; set; }
    }
}
