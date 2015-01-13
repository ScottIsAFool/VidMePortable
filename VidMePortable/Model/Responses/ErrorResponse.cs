using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class ErrorResponse : Response
    {
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
