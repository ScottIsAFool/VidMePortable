using VidMePortable.Attributes;

namespace VidMePortable.Model
{
    public enum NotificationType
    {
        // channel-subscribed - When someone subscribes to a channel of which you are a moderator.
        [Description("channel-subscribed")]
        ChannelSubscribed,
        // comment-replied-to - When someone replied to your comment (currently disabled)
        [Description("comment-replied-to")]
        CommentReply,
        // user-subscribed - When someone follows you
        [Description("user-subscribed")]
        UserSubscribed,
        // user-welcome - After you sign up
        [Description("user-welcome")]
        UserWelcome,
        // video-commented - When someone comments on your video
        [Description("video-commented")]
        VideoComment,
        // video-upvoted - When someone likes (up votes) your video
        [Description("video-upvoted")]
        VideoUpVoted
    }
}
