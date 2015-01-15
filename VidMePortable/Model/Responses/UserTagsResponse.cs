using System.Collections.Generic;
using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class UserTagsResponse : Response
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("users")]
        public List<UserTag> UserTags { get; set; }
    }

}
