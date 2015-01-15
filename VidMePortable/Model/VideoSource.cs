using VidMePortable.Attributes;

namespace VidMePortable.Model
{
    public enum VideoSource
    {
        [Description("computer")]
        Computer,
        [Description("library")]
        Library,
        [Description("camera")]
        Camera,
        [Description("shareintent")]
        ShareIntent,
        [Description("webcam")]
        WebCam
    }
}