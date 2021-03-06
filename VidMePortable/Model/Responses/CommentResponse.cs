﻿using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class CommentResponse : Response
    {
        [JsonProperty("comment")]
        public Comment Comment { get; set; }

        [JsonProperty("vote")]
        public ViewerVote ViewerVote { get; set; }
    }
}
