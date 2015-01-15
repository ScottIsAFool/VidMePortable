using System;
using System.Collections.Generic;
using System.Linq;

namespace VidMePortable.Extensions
{
    internal static class DictionaryExtensions
    {
        internal static void AddIfNotNull(this Dictionary<string, string> postData, string key, int? item)
        {
            if (item.HasValue)
            {
                postData.Add(key, item.Value.ToString());
            }
        }

        internal static void AddIfNotNull(this Dictionary<string, string> postData, string key, double? item)
        {
            if (item.HasValue)
            {
                postData.Add(key, item.Value.ToString());
            }
        }

        internal static void AddIfNotNull(this Dictionary<string, string> postData, string key, DateTime? item)
        {
            if (item.HasValue)
            {
                postData.Add(key, item.Value.ToString());
            }
        }

        internal static void AddIfNotNull<TEnum>(this Dictionary<string, string> postData, string key, TEnum? item) where TEnum : struct
        {
            if (item.HasValue)
            {
                postData.Add(key, item.Value.GetDescription().ToLower());
            }
        }

        internal static void AddIfNotNull(this Dictionary<string, string> postData, string key, bool? item)
        {
            if (item.HasValue)
            {
                postData.Add(key, item.Value.ToString());
            }
        }

        internal static void AddIfNotNull(this Dictionary<string, string> postData, string key, string item)
        {
            if (!string.IsNullOrEmpty(item))
            {
                postData.Add(key, item);
            }
        }

        internal static void AddIfNotNull<T>(this Dictionary<string, string> postData, string key, List<T> item)
        {
            if (item != null && item.Any())
            {
                postData.Add(key, string.Join(",", item));
            }
        }
    }
}