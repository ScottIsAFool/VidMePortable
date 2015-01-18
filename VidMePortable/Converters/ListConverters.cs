using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace VidMePortable.Converters
{
    public class ObjectToArrayConverter<T> : CustomCreationConverter<IEnumerable<T>> where T : new()
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject;
            List<T> results = new List<T>();

            // object is an array
            if (reader.TokenType == JsonToken.StartArray)
            {
                return serializer.Deserialize<IEnumerable<T>>(reader);
            }
            else if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            else if (reader.TokenType == JsonToken.String)
            {
                return null;
            }

            try
            {
                jObject = JObject.Load(reader);
            }
            catch
            {
                return null;
            }

            // Populate the object properties
            foreach (KeyValuePair<string, JToken> item in jObject)
            {
                results.Add(
                  serializer.Deserialize<T>(item.Value.CreateReader())
                );
            }

            return results;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<T> Create(Type objectType)
        {
            return new List<T>();
        }
    }
}
