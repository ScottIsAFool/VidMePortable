using Newtonsoft.Json;

namespace VidMePortable.Model
{
    public class Response
    {
        [JsonProperty("status")]
        public bool Status { get; set; }
    }
}
