using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class VoteResponse : Response
    {
        [JsonProperty("vote")]
        public ViewerVote ViewerVote { get; set; }
    }
}