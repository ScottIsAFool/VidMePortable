using Newtonsoft.Json;
using PropertyChanged;

namespace VidMePortable.Model
{
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
        public object ParentCommentId { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("date_created")]
        public string DateCreated { get; set; }

        [JsonProperty("made_at_time")]
        public object MadeAtTime { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }
    }
}