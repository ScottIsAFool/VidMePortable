using PropertyChanged;

namespace VidMePortable.Model.Requests
{
    [ImplementPropertyChanged]
    public class LocationRequest
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string GeofenceId { get; set; }
        public double? Distance { get; set; }
        public double? From { get; set; }
        public double? To { get; set; }
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public LocationOrderBy? LocationOrderBy { get; set; }
    }
}