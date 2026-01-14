using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text;
using log4net.Appender;
using log4net.Core;
using Serilog;

namespace StaxiLogging.Wrappers
{
    public class SerilogLog4NetAppender : AppenderSkeleton
    {
        private readonly Serilog.ILogger _logger;
        public SerilogLog4NetAppender(Serilog.ILogger logger)
        {
            _logger = logger;
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null) return;
            var msg = loggingEvent.RenderedMessage;
            var ex = loggingEvent.ExceptionObject;

            switch (loggingEvent.Level.Name)
            {
                case "DEBUG":
                        _logger.Debug(ex, msg);
                    break;
                case "INFO":
                    _logger.Information(ex, msg);
                    break;
                case "WARN":
                    _logger.Warning(ex, msg);
                    break;
                case "ERROR":
                    _logger.Error(ex, msg);
                    break;
                case "FATAL":
                    _logger.Fatal(ex, msg);
                    break;
                default:
                    _logger.Information(ex, msg);
                    break;
            }
        }
    }
}
