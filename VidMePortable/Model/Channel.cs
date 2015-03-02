using System;
using System.Diagnostics;
using Newtonsoft.Json;
using PropertyChanged;
using VidMePortable.Converters;

namespace VidMePortable.Model
{
    [DebuggerDisplay("Title: {Title}, Id: {ChannelId}")]
    [ImplementPropertyChanged]
    public class Channel
    {
        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("full_url")]
        public string FullUrl { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonProperty("cover_url")]
        public string CoverUrl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("date_created")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? DateCreated { get; set; }

        [JsonProperty("is_default")]
        public bool IsDefault { get; set; }

        [JsonProperty("hide_suggest")]
        public bool HideSuggest { get; set; }

        [JsonProperty("show_unmoderated")]
        public bool ShowUnmoderated { get; set; }

        [JsonProperty("nsfw")]
        public bool Nsfw { get; set; }

        [JsonProperty("follower_count")]
        public int FollowerCount { get; set; }

        [JsonProperty("video_count")]
        public int VideoCount { get; set; }
        
        [JsonProperty("colors")]
        public string Colors { get; set; }
    }
}