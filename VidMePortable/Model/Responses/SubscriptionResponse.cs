using Newtonsoft.Json;

namespace VidMePortable.Model.Responses
{
    public class SubscriptionResponse : Response
    {
        [JsonProperty("subscription_id")]
        public string SubscriptionId { get; set; }
    }
}