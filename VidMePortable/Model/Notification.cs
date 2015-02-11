using System;
using System.Diagnostics;
using Newtonsoft.Json;
using PropertyChanged;
using VidMePortable.Converters;

namespace VidMePortable.Model
{
    [DebuggerDisplay("Text: {Text}, Id: {NotificationId}")]
    [ImplementPropertyChanged]
    public class Notification
    {
        [JsonProperty("notification_id")]
        public string NotificationId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(EnumConverter<NotificationType>))]
        public NotificationType Type { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("actor")]
        public User User { get; set; }

        [JsonProperty("read")]
        public bool Read { get; set; }

        [JsonProperty("date_created")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? DateCreated { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}