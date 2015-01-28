using System.Collections.Generic;
using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class CommentsResponse : Response
    {
        [JsonProperty("page")]
        public Page Page { get; set; }

        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; }

        [JsonProperty("comments")]
        public List<Comment> Comments { get; set; }
    }

}
