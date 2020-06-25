﻿using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace PillsBot.Server
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = CreateLoggerConfiguration().CreateLogger();

            try
            {
                Log.Information("Starting host");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static LoggerConfiguration CreateLoggerConfiguration()
        {
            LoggerConfiguration configuration = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Environment", Env.Name)
                .Enrich.WithProperty("Version", new
                {
                    Server = typeof(Program).Assembly.GetName().Version.ToString(3)
                }, true);

            configuration = Env.IsDevelopment
                ? configuration.MinimumLevel.Debug()
                : configuration.MinimumLevel.Information();

            return configuration.WriteTo.Console();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder<Startup>(args)
                .UseSerilog();
        }
    }
}
