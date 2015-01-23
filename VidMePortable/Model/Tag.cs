using System.Diagnostics;
using Newtonsoft.Json;
using PropertyChanged;

namespace VidMePortable.Model
{
    [DebuggerDisplay("Text: {Text}, Id: {TagId}")]
    [ImplementPropertyChanged]
    public class Tag
    {
        [JsonProperty("tag_id")]
        public string TagId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("use_count")]
        public string UseCount { get; set; }
    }
}