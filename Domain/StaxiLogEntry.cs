using Nest;
using System;
using System.Collections.Generic;

namespace StaxiLogging.Domain
{
    public class StaxiLogEntry
    {
        [PropertyName("@timestamp")]
        public DateTime Timestamp { get; set; }

        [PropertyName("level")]
        public string Level { get; set; }

        [PropertyName("message")]
        public string Message { get; set; }

        [PropertyName("RequestPath")]
        public string RequestPath { get; set; }

        [PropertyName("Application")]
        public string Application { get; set; }

        [PropertyName("Environment")]
        public string Environment { get; set; }

        [PropertyName("SourceContext")]
        public string SourceContext { get; set; }

        [PropertyName("ActionId")]
        public string ActionId { get; set; }

        [PropertyName("ActionName")]
        public string ActionName { get; set; }

        [PropertyName("RequestId")]
        public string RequestId { get; set; }

        [PropertyName("ConnectionId")]
        public string ConnectionId { get; set; }

        [PropertyName("THIRDPARTY_HISTORY")]
        public ThirdPartyHistory ThirdPartyHistory { get; set; }

        public Dictionary<string, object> Fields { get; set; }
    }

    public class RequestInfo
    {
        [PropertyName("headers")]
        public Dictionary<string, string> Headers { get; set; }

        [PropertyName("body")]
        public object Body { get; set; }
    }

    public class ResponseInfo
    {
        [PropertyName("body")]
        public object Body { get; set; }
    }

    public class ThirdPartyHistory
    {
        [PropertyName("type")]
        public string Type { get; set; }

        [PropertyName("traceId")]
        public string TraceId { get; set; }

        [PropertyName("requestId")]
        public string RequestId { get; set; }

        [PropertyName("method")]
        public string Method { get; set; }

        [PropertyName("path")]
        public string Path { get; set; }

        [PropertyName("statusCode")]
        public int StatusCode { get; set; }

        [PropertyName("request")]
        public RequestInfo Request { get; set; }

        [PropertyName("response")]
        public ResponseInfo Response { get; set; }

        [PropertyName("exception")]
        public object Exception { get; set; }

        [PropertyName("exceptionDetail")]
        public object ExceptionDetail { get; set; }
    }
}
