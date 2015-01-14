using VidMePortable.Attributes;

namespace VidMePortable.Model
{
    public enum Scope
    {
        [Description(":auth_management")]
        AuthManagement,
        [Description(":client_management")]
        ClientManagement,
        [Description("account")]
        Account,
        [Description("basic")]
        Basic,
        [Description("channels")]
        Channels,
        [Description("comments")]
        Comments,
        [Description("videos")]
        Videos,
        [Description("video_upload")]
        VideoUpload,
        [Description("votes")]
        Votes
    }
}
