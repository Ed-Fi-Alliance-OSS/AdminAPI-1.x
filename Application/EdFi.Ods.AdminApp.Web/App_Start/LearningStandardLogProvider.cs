// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using log4net;
using Microsoft.Extensions.Logging;

namespace EdFi.Ods.AdminApp.Web
{
    public class LearningStandardLogProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new LearningStandardLogger();
        }

        public void Dispose()
        {

        }
    }

    public class LearningStandardLogger : ILogger
    {
        private readonly ILog _logger = LogManager.GetLogger("LearningStandardLog");

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var formattedMsg = formatter(state, exception);

            switch (logLevel)
            {
                case LogLevel.Trace:
                    _logger.Debug(formattedMsg);
                    break;
                case LogLevel.Debug:
                    _logger.Debug(formattedMsg);
                    break;
                case LogLevel.Information:
                    _logger.Info(formattedMsg);
                    break;
                case LogLevel.Warning:
                    _logger.Warn(formattedMsg);
                    break;
                case LogLevel.Error:
                    _logger.Error(formattedMsg);
                    break;
                case LogLevel.Critical:
                    _logger.Fatal(formattedMsg);
                    break;
                case LogLevel.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return _logger.IsDebugEnabled;
                case LogLevel.Debug:
                    return _logger.IsDebugEnabled;
                case LogLevel.Information:
                    return _logger.IsInfoEnabled;
                case LogLevel.Warning:
                    return _logger.IsWarnEnabled;
                case LogLevel.Error:
                    return _logger.IsErrorEnabled;
                case LogLevel.Critical:
                    return _logger.IsFatalEnabled;
                case LogLevel.None:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null);
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}