using System.Collections.Generic;
using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class NotificationsResponse : Response
    {
        [JsonProperty("notifications")]
        public List<Notification> Notifications { get; set; }
    }

}
