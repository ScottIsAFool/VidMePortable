using System.Collections.Generic;
using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class CommentsResponse : Response
    {
        [JsonProperty("comments")]
        public List<Comment> Comments { get; set; }
    }
}