using System;
using System.IO;
using System.Threading.Tasks;

namespace VidMePortable.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<string> ToBase64String(this Stream stream)
        {
            if (stream == null)
            {
                return string.Empty;
            }

            using (var memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                var bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
