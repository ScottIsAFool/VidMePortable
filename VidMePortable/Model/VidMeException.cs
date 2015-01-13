using System;
using System.Net;

namespace VidMePortable.Model
{
    public class VidMeException : Exception
    {
        public VidMeException(HttpStatusCode statusCode, string error)
        {
            StatusCode = statusCode;
            Error = error;
        }

        public HttpStatusCode StatusCode { get; set; }
        public string Error { get; set; }
    }
}
