using Microsoft.Extensions.DependencyInjection;
using Nest;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.File;
using StaxiLogging;
using StaxiLogging.Services;
using StaxiLogging.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace StaxiLogging.src
{
    public static class SerilogElasticExtension
    {
        public static LoggerConfiguration UseElasticLoggingConfig(this LoggerConfiguration logger, ElasticLoggingOption options)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var fullPath = Path.Combine(baseDir, options.PathFileSinkFail);
            // Đảm bảo thư mục Logs tồn tại, nếu không có IIS sẽ không tự tạo và gây lỗi
            var logDirectory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(logDirectory) && !Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            var failureLogger = new LoggerConfiguration()
                    .WriteTo.File(
                        path: fullPath, 
                        rollingInterval: RollingInterval.Day, 
                        retainedFileCountLimit: 31,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                    .CreateLogger();

            return logger
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)

                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", options.ApplicationName)
                .Enrich.WithProperty("Environment", options.EnvironmentName)
                .Enrich.WithProperty("Hostname", Environment.MachineName)
                .Enrich.WithProperty("Containername", Environment.GetEnvironmentVariable("HOSTNAME"))
                .Enrich.WithProperty("Pod", Environment.GetEnvironmentVariable("POD_NAME"))
                .Enrich.WithProperty("Node", Environment.GetEnvironmentVariable("NODE_NAME"))
                  .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(options.Uri))
                  {
                      AutoRegisterTemplate = options.AutoRegisterTemplate,
                      IndexFormat = options.IndexFormat,    
                      NumberOfReplicas = options.NumberOfReplicas,
                      NumberOfShards = options.NumberOfShards,
                      AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv8,
                      MinimumLogEventLevel = options.MiniLogLevel,
                      BatchPostingLimit = options.BatchPostingLimit,

                      ModifyConnectionSettings = conn =>
                        conn.BasicAuthentication(options.User,
                         options.Password
                        ),

                      TypeName = options.TypeName,

                      // Tạo với ILM
                      CustomFormatter = new ElasticsearchJsonFormatter(
                        renderMessage: options.RenderMessage,
                        inlineFields: options.InlineFields
                      ),
                      EmitEventFailure = EmitEventFailureHandling.WriteToFailureSink,
                      FailureSink = failureLogger
                  });
        }

        public static IServiceCollection AddStaxiLogRead(this IServiceCollection services, ElasticLoggingOption options)
        {

            string indexPattern = System.Text.RegularExpressions.Regex.Replace(options.IndexFormat, @"\{0:.*?\}", "*");


            var connection = new ConnectionSettings(new Uri(options.Uri))
                .BasicAuthentication(options.User, options.Password);

            var client = new ElasticClient(connection);

            services.AddSingleton<IElasticClient>(client);

            services.AddScoped<IStaxiLogReader>(sp =>
            {
                return new StaxiLogReader(client, indexPattern);
            });

            return services;
        }

        public static void RedirectLog4Net(this Serilog.ILogger logger)
        {
            var repo = log4net.LogManager.GetRepository();
            var appender = new SerilogLog4NetAppender(logger);
            log4net.Config.BasicConfigurator.Configure(appender);
        }
        public static void RedirectNLog(this Serilog.ILogger logger)
        {
            var config = NLog.LogManager.Configuration ?? new NLog.Config.LoggingConfiguration();
            var target = new SerilogNLogTarget(logger);
            config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, target);
            NLog.LogManager.Configuration = config;
        }

    }
}
