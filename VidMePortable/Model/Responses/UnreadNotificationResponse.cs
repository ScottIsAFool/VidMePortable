using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class UnreadNotificationResponse : Response
    {
        [JsonProperty("unreadCount")]
        public string UnreadCount { get; set; }
    }
}
