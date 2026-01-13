using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Serilog.Sinks.File;
namespace StaxiLogging.src
{
    public static class SerilogElasticExtension
    {
        public static LoggerConfiguration UseElasticLoggingConfig(this LoggerConfiguration logger, ElasticLoggingOption options)
        {
            return logger
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)

                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", options.ApplicationName)
                .Enrich.WithProperty("Environment", options.EnvironmentName)

                  .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(options.Uri))
                  {
                      AutoRegisterTemplate = options.AutoRegisterTemplate,
                      IndexFormat = options.IndexFormat,
                      NumberOfReplicas = options.NumberOfReplicas,
                      NumberOfShards = options.NumberOfShards,
                      AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                      MinimumLogEventLevel = options.MiniLogLevel,
                      BatchPostingLimit = options.BatchPostingLimit,

                      ModifyConnectionSettings = conn =>
                        conn.BasicAuthentication(options.User,
                         options.Password
                        ),

                      EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog |
                       EmitEventFailureHandling.WriteToFailureSink,

                      FailureSink = new FileSink(
                        options.PathFileSinkFail,
                        new Serilog.Formatting.Json.JsonFormatter(),
                        null
                        )
                  });
        }

    }
}
