using Newtonsoft.Json;
using PropertyChanged;

namespace VidMePortable.Model.Responses
{
    [ImplementPropertyChanged]
    public class CreateAppResponse : Response
    {
        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("client_secret")]
        public string ClientSecret { get; set; }

        [JsonProperty("application")]
        public Application Application { get; set; }
    }

}
