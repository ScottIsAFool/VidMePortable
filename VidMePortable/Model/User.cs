﻿using Newtonsoft.Json;
using PropertyChanged;

namespace VidMePortable.Model
{
    [ImplementPropertyChanged]
    public class User
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("cover")]
        public string Cover { get; set; }

        [JsonProperty("cover_url")]
        public string CoverUrl { get; set; }

        [JsonProperty("video_views")]
        public string VideoViews { get; set; }

        [JsonProperty("likes_count")]
        public string LikesCount { get; set; }
    }
}