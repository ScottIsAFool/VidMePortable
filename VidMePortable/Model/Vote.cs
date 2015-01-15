using VidMePortable.Attributes;

namespace VidMePortable.Model
{
    public enum Vote
    {
        [Description("1")]
        Up,
        [Description("-1")]
        Down,
        [Description("0")]
        Neutral
    }
}