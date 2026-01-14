using NLog;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaxiLogging.Wrappers
{
    [Target("SerilogTarget")]
    public class SerilogNLogTarget : TargetWithLayout
    {
        private readonly Serilog.ILogger _logger;

        public SerilogNLogTarget(Serilog.ILogger logger)
        {
            _logger = logger;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (logEvent == null) return;
        
            var msg = this.Layout.Render(logEvent);

            switch (logEvent.Level.Name)
            {
                case "DEBUG":
                    _logger.Debug(msg);
                    break;
                case "Info":
                    _logger.Information(msg);
                    break;
                case "Warn":
                    _logger.Warning(msg);
                    break;
                case "Error":
                    _logger.Error(logEvent.Exception, msg);
                    break;
                case "Fatal":
                    _logger.Fatal(logEvent.Exception, msg);
                    break;
                default:
                    _logger.Information(msg);
                    break;
            }
        }
    }
}
