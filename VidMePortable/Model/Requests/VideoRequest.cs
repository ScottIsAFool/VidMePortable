using System.IO;
using PropertyChanged;

namespace VidMePortable.Model.Requests
{
    [ImplementPropertyChanged]
    public class VideoRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Stream ThumbnailStream { get; set; }
        public VideoSource? VideoSource { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string FourSquarePlaceId { get; set; }
        public string FourSquarePlaceName { get; set; }
        public bool? IsPrivate { get; set; }
        public string ChannelId { get; set; }
        public float? VideoSize { get; set; }
        public string FileName { get; set; }
        /// <summary>
        /// Gets or sets if the video is NSFW.
        /// NOTE: Will only allow you to say video is NSFW, can't change a video from NSFW -> SFW
        /// </summary>
        /// <value>
        /// The is NSFW.
        /// </value>
        public bool? IsNsfw { get; set; }
    }
}