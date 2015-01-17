using Newtonsoft.Json;
using PropertyChanged;

namespace VidMePortable.Model.Responses
{
    [ImplementPropertyChanged]
    public class AuthResponse : Response
    {
        [JsonProperty("auth")]
        public Auth Auth { get; set; }

        [JsonProperty("user")]
        public User User { get; set; }
    }

}
