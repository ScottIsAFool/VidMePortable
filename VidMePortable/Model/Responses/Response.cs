using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class Response
    {
        [JsonProperty("status")]
        internal bool Status { get; set; }
    }
}
