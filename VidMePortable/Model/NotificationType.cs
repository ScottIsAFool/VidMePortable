using VidMePortable.Attributes;

namespace VidMePortable.Model
{
    public enum NotificationType
    {
        /// <summary>
        /// channel-subscribed - When someone subscribes to a channel of which you are a moderator.
        /// </summary>
        [Description("channel-subscribed")]
        ChannelSubscribed,
        
        /// <summary>
        /// comment-replied-to - When someone replied to your comment (currently disabled)
        /// </summary>
        [Description("comment-replied-to")]
        CommentReply,
        
        /// <summary>
        ///user-subscribed - When someone follows you
        /// </summary>
        [Description("user-subscribed")]
        UserSubscribed,
        
        /// <summary>
        /// user-welcome - After you sign up
        /// </summary>
        [Description("user-welcome")]
        UserWelcome,
        
        /// <summary>
        /// video-commented - When someone comments on your video
        /// </summary>
        [Description("video-commented")]
        VideoComment,
        
        /// <summary>
        /// video-upvoted - When someone likes (up votes) your video
        /// </summary>
        [Description("video-upvoted")]
        VideoUpVoted
    }
}
