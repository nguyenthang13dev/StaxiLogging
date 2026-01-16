using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaxiLogging.Domain
{
    public class StaxiLogEntry
    {
        [JsonProperty("@timestamp")] // map đúng tên ES
        public DateTime Timestamp { get; set; }

        [JsonProperty("level")]
        public string Level { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        // Các field “cố định” khác
        public string RequestPath { get; set; }
        public string Application { get; set; }
        public string Environment { get; set; }
        public string SourceContext { get; set; }
        public string ActionId { get; set; }
        public string ActionName { get; set; }
        public string RequestId { get; set; }
        public string ConnectionId { get; set; }

        // Field dynamic / enrich
        public Dictionary<string, object> Fields { get; set; }

     
    }
}
