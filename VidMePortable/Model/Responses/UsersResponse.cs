using System.Collections.Generic;
using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class UsersResponse : Response
    {
        [JsonProperty("page")]
        public Page Page { get; set; }

        [JsonProperty("users")]
        public List<User> Users { get; set; }
    }
}