using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VidMePortable.Converters
{
    public class DateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }

            var date = (DateTime) value;
            writer.WriteValue(date);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null || reader.Value.ToString() == "0000-00-00 00:00:00" || string.IsNullOrEmpty(reader.Value.ToString()))
            {
                return null;
            }

            return DateTime.Parse(reader.Value.ToString());
        }
    }
}
