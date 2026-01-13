using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaxiLogging.src
{
    public class ElasticLoggingOption
    {
        public string Uri { get; set; } = "http://localhost:9200";
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = string.Empty;
        public string IndexFormat { get; set; } = "staxi-log-app-{0:yyyy.MM.dd}";
        public string ApplicationName { get; set; } = "StaxiApp";
        public bool EnableLogConsole { get; set; } = false;
        // Mini log
        public LogEventLevel MiniLogLevel { get; set; } = LogEventLevel.Information;
        public string PathFileSinkFail { get; set; }
        //Index
        public bool AutoRegisterTemplate { get; set; } = false;
        public int NumberOfReplicas { get; set; } = 1;
        public int NumberOfShards { get; set; } = 1;
        public int BatchPostingLimit { get; set; }


    }

}
