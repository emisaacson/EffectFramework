﻿using EffectFramework.Core;
using Microsoft.Extensions.Configuration;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
namespace EffectFramework.Test
{
    /// <summary>
    /// Basic test for wiring up the logging service.
    /// </summary>
    public class LoggerTests : IDisposable
    {
        private string _BasePath = null;
        private string BasePath
        {
            get
            {
                if (_BasePath == null)
                {
                    _BasePath = Directory.GetCurrentDirectory();
                }
                return _BasePath;
            }
        }

        public LoggerTests()
        {
            Configure.RegisterLoggingProvider<MemoryLoggingProvider>();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Configure.PersistenceConnectionString = configuration["Data:DefaultConnection:ConnectionString"];
        }

        [Fact]
        public void TestLogging()
        {
            using (IKernel Kernel = new StandardKernel(new Configure()))
            {
                ILoggingProvider Log = Kernel.Get<ILoggingProvider>();

                Log.Debug("testdebug");
                Log.Warn("testwarn");
                Log.Fatal("testfatal");
                
                Assert.Equal("testdebug", MemoryLoggingProvider.Logs[MemoryLoggingProvider.LogLevels.Debug][0]);
                Assert.Equal("testwarn", MemoryLoggingProvider.Logs[MemoryLoggingProvider.LogLevels.Warn][0]);
                Assert.Equal("testfatal", MemoryLoggingProvider.Logs[MemoryLoggingProvider.LogLevels.Fatal][0]);
            }
        }

        public void Dispose()
        {

        }

        public class MemoryLoggingProvider : ILoggingProvider
        {
            public enum LogLevels {
                Trace,
                Info,
                Debug,
                Warn,
                Error,
                Fatal };

            public static Dictionary<LogLevels, List<string>> Logs = new Dictionary<LogLevels, List<string>>()
            {
                { LogLevels.Trace, new List<string>() },
                { LogLevels.Info, new List<string>() },
                { LogLevels.Debug, new List<string>() },
                { LogLevels.Warn, new List<string>() },
                { LogLevels.Error, new List<string>() },
                { LogLevels.Fatal, new List<string>() },
            };

            public void Debug(Exception e, string Template, params object[] objs)
            {
                Logs[LogLevels.Debug].Add(Template);
            }

            public void Debug(string Template, params object[] objs)
            {
                Logs[LogLevels.Debug].Add(Template);
            }

            public void Error(Exception e, string Template, params object[] objs)
            {
                Logs[LogLevels.Error].Add(Template);
            }

            public void Error(string Template, params object[] objs)
            {
                Logs[LogLevels.Error].Add(Template);
            }

            public void Fatal(Exception e, string Template, params object[] objs)
            {
                Logs[LogLevels.Fatal].Add(Template);
            }

            public void Fatal(string Template, params object[] objs)
            {
                Logs[LogLevels.Fatal].Add(Template);
            }

            public void Info(Exception e, string Template, params object[] objs)
            {
                Logs[LogLevels.Info].Add(Template);
            }

            public void Info(string Template, params object[] objs)
            {
                Logs[LogLevels.Info].Add(Template);
            }

            public void Trace(Exception e, string Template, params object[] objs)
            {
                Logs[LogLevels.Trace].Add(Template);
            }

            public void Trace(string Template, params object[] objs)
            {
                Logs[LogLevels.Trace].Add(Template);
            }

            public void Warn(Exception e, string Template, params object[] objs)
            {
                Logs[LogLevels.Warn].Add(Template);
            }

            public void Warn(string Template, params object[] objs)
            {
                Logs[LogLevels.Warn].Add(Template);
            }
        }
    }
}
