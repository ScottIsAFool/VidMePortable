using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class VideoResponse : Response
    {
        [JsonProperty("video")]
        public Video Video { get; set; }
    }
}