using System;
using System.Diagnostics;
using Newtonsoft.Json;
using PropertyChanged;
using VidMePortable.Converters;

namespace VidMePortable.Model
{
    [DebuggerDisplay("Value: {Value}, Id: {VoteId}")]
    [ImplementPropertyChanged]
    public class ViewerVote
    {
        [JsonProperty("vote_id")]
        public string VoteId { get; set; }

        [JsonProperty("video_id")]
        public string VideoId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("value")]
        public int? Value { get; set; }

        [JsonProperty("date_created")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? DateCreated { get; set; }

        [JsonProperty("date_modified")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? DateModified { get; set; }
    }
}
