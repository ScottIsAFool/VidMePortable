using System.Diagnostics;
using Newtonsoft.Json;
using PropertyChanged;

namespace VidMePortable.Model
{
    [DebuggerDisplay("Username: {Username}, Id: {UserId}")]
    [ImplementPropertyChanged]
    public class User
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("full_url")]
        public string FullUrl { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("cover")]
        public string Cover { get; set; }

        [JsonProperty("cover_url")]
        public string CoverUrl { get; set; }

        [JsonProperty("follower_count")]
        public int FollowerCount { get; set; }

        [JsonProperty("likes_count")]
        public string LikesCount { get; set; }

        [JsonProperty("video_count")]
        public int VideoCount { get; set; }

        [JsonProperty("video_views")]
        public string VideoViews { get; set; }

        [JsonProperty("videos_scores")]
        public float VideosScores { get; set; }

        [JsonProperty("bio")]
        public string Bio { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}