using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class Response
    {
        [JsonProperty("status")]
        public bool Status { get; set; }
    }
}
