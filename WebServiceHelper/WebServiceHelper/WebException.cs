using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Runtime;
using System.Runtime.Serialization;
using System.Net;
using System.Text;

namespace GregWebServices
{
    public class WebException : Exception
    {
        private string _stackTrace { get; set; }
        private HttpStatusCode _statusCode { get; set; }
        public WebException(WebError error, HttpStatusCode statusCode) : this(error.Error, statusCode) { _stackTrace = error.Stack; }
        public WebException(string message, HttpStatusCode statusCode) : base(string.IsNullOrWhiteSpace(message) ? statusCode.ToString() : $"{statusCode.ToString()} - {message}") { _statusCode = statusCode; }
        public override string StackTrace => _stackTrace ?? base.StackTrace;
        public HttpStatusCode StatusCode => _statusCode;
        public override string ToString()
        {
            var description = new StringBuilder();
            description.AppendLine($"{GetType().Name}: {Message}");
            description.AppendLine(StackTrace);

            if (!string.IsNullOrWhiteSpace(_stackTrace))
            {
                description.AppendLine($"{Environment.NewLine}--- End of web exception stack trace ---{Environment.NewLine}");
                description.Append(base.StackTrace);
            }

            return description.ToString();
        }
    }
}
