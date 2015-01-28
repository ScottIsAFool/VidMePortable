using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using PropertyChanged;
using VidMePortable.Converters;
using VidMePortable.Model.Responses;

namespace VidMePortable.Model
{
    [DebuggerDisplay("Url: {FullUrl}, Id: {VideoId}, UserId: {UserId}")]
    [ImplementPropertyChanged]
    public class Video
    {
        [JsonProperty("video_id")]
        public string VideoId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("full_url")]
        public string FullUrl { get; set; }

        [JsonProperty("embed_url")]
        public string EmbedUrl { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("complete")]
        public string Complete { get; set; }

        [JsonProperty("complete_url")]
        public string CompleteUrl { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("duration")]
        public double? Duration { get; set; }

        [JsonProperty("height")]
        public double? Height { get; set; }

        [JsonProperty("width")]
        public double? Width { get; set; }

        [JsonProperty("date_created")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? DateCreated { get; set; }

        [JsonProperty("date_stored")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? DateStored { get; set; }

        [JsonProperty("date_completed")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? DateCompleted { get; set; }

        [JsonProperty("comment_count")]
        public int CommentCount { get; set; }

        [JsonProperty("view_count")]
        public int ViewCount { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("nsfw")]
        public bool Nsfw { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("thumbnail_gif")]
        public string ThumbnailGif { get; set; }

        [JsonProperty("thumbnail_gif_url")]
        public string ThumbnailGifUrl { get; set; }

        [JsonProperty("storyboard")]
        public string Storyboard { get; set; }

        [JsonProperty("score")]
        public int Score { get; set; }

        [JsonProperty("likes_count")]
        public int LikesCount { get; set; }

        [JsonProperty("channel_id")]
        public string ChannelId { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("private")]
        public bool Private { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("place_name")]
        public string PlaceName { get; set; }

        [JsonProperty("colors")]
        public string Colors { get; set; }

        [JsonProperty("clip_url")]
        public string ClipUrl { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }

        [JsonProperty("formats")]
        public List<VideoFormat> VideoFormats { get; set; }
    }
}