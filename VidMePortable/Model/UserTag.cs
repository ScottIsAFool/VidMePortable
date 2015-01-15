using Newtonsoft.Json;
using PropertyChanged;

namespace VidMePortable.Model
{
    [ImplementPropertyChanged]
    public class UserTag
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }
}