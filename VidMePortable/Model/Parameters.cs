using Newtonsoft.Json;

namespace VidMePortable.Model
{
    public class Parameters
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }
    }
}