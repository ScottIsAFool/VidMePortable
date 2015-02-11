using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VidMePortable.Extensions;

namespace VidMePortable.Converters
{
    public class EnumConverter<TEnum> : JsonConverter
    {
        private static Dictionary<TEnum, string> _listTypeDictionary;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var enumType = (TEnum)value;
            writer.WriteValue(enumType.GetDescription());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;
            if (_listTypeDictionary == null)
            {
                var types = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
                _listTypeDictionary = types.Select(x => new KeyValuePair<TEnum, string>(x, x.GetDescription().ToLower())).ToDictionary(x => x.Key, x => x.Value);
            }

            if (reader.ValueType == typeof(string))
            {
                var result = _listTypeDictionary.FirstOrDefault(x => x.Value == (reader.Value.ToString()).ToLower());

                return result.Key;
            }

            var num = int.Parse(reader.Value.ToString());

            return (TEnum)Enum.ToObject(typeof(TEnum), num);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}
