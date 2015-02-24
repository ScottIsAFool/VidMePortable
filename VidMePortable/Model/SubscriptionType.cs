using VidMePortable.Attributes;

namespace VidMePortable.Model
{
    public enum SubscriptionType
    {
        [Description("apn")]
        Apple,
        [Description("gcm")]
        Android,
        [Description("web")]
        Web
    }
}