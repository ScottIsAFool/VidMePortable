using System.Collections.Generic;
using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class NotificationsResponse : Response
    {
        [JsonProperty("page")]
        public Page Page { get; set; }

        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; }

        [JsonProperty("notifications")]
        public List<Notification> Notifications { get; set; }
    }
}
