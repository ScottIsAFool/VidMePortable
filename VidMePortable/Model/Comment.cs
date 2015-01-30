using System;
using System.Diagnostics;
using Newtonsoft.Json;
using PropertyChanged;
using VidMePortable.Converters;

namespace VidMePortable.Model
{
    [DebuggerDisplay("Body: {Body}, Id: {CommentId}")]
    [ImplementPropertyChanged]
    public class Comment
    {
        [JsonProperty("comment_id")]
        public string CommentId { get; set; }

        [JsonProperty("video_id")]
        public string VideoId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("parent_comment_id")]
        public string ParentCommentId { get; set; }

        [JsonProperty("full_url")]
        public string FullUrl { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("date_created")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? DateCreated { get; set; }

        [JsonProperty("made_at_time")]
        public double? MadeAtTime { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }
}