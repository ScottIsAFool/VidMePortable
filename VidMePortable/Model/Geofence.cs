using System;
using Newtonsoft.Json;
using PropertyChanged;
using VidMePortable.Converters;

namespace VidMePortable.Model
{
    [ImplementPropertyChanged]
    public class Geofence
    {
        [JsonProperty("geofence_id")]
        public string GeofenceId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("radius")]
        public int Radius { get; set; }

        [JsonProperty("radius_unit")]
        public string RadiusUnit { get; set; }

        [JsonProperty("date_created")]
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime? DateCreated { get; set; }
    }
}