using System.IO;
using PropertyChanged;

namespace VidMePortable.Model.Responses
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
    }
}