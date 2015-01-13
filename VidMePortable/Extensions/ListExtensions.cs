using System.Collections.Generic;
using System.Linq;

namespace VidMePortable.Extensions
{
    public static class ListExtensions
    {
        public static bool IsNullOrEmpty<T>(this List<T> list)
        {
            return list == null || !list.Any();
        }

        public static string ToQueryString(this Dictionary<string, string> postData)
        {
            var items = postData.Select(x => string.Format("{0}={1}", x.Key, x.Value));

            return string.Join("&", items);
        }
    }
}
