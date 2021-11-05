using System;
using System.Net;

namespace Yggdrasil.HttpExceptions
{
    public class HttpResponseException : Exception
    {
        public int Status { get; set; } = 500;
        public object Value { get; set; }

        public HttpResponseException(HttpStatusCode code, object value)
        {
            Status = (int)code;
            Value = value;
        }
    }
}
