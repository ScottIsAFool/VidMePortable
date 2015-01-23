using VidMePortable.Attributes;

namespace VidMePortable.Model
{
    public enum LocationOrderBy
    {
        [Description("likes_count")]
        LikesCount,
        [Description("hot_score")]
        HotScore
    }
}