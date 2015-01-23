using System.Diagnostics;
using Newtonsoft.Json;
using PropertyChanged;

namespace VidMePortable.Model
{
    [DebuggerDisplay("Name: {Name}, Id: {ApplicationId}")]
    [ImplementPropertyChanged]
    public class Application
    {
        [JsonProperty("application_id")]
        public string ApplicationId { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("website")]
        public string Website { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("organization")]
        public string Organization { get; set; }

        [JsonProperty("redirect_uri_prefix")]
        public string RedirectUriPrefix { get; set; }
    }
}