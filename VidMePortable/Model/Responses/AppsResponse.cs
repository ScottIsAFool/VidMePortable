using System.Collections.Generic;
using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class AppsResponse : Response
    {
        [JsonProperty("applications")]
        public List<Application> Applications { get; set; }
    }
}
