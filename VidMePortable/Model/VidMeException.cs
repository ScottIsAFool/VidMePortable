using System;
using System.Net;
using VidMePortable.Model.Responses;

namespace VidMePortable.Model
{
    public class VidMeException : Exception
    {
        public VidMeException(HttpStatusCode statusCode, ErrorResponse error)
        {
            StatusCode = statusCode;
            Error = error;
        }

        public ErrorResponse Error { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
