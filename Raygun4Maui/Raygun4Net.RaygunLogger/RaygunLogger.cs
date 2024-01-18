using Microsoft.Extensions.Logging;
using Raygun4Maui;
using Raygun4Net.BuildPlatforms;

// Namespace is different, doesn't include Raygun4Maui because we may want to move this to Raygun4Net
// ReSharper disable once CheckNamespace
namespace Raygun4Net.RaygunLogger
{
    public sealed class RaygunLogger : ILogger
    {
        private readonly string _name;
        private readonly RaygunLoggerConfiguration _raygunLoggerConfiguration;

        public RaygunLogger(string name, RaygunLoggerConfiguration raygunLoggerConfiguration) =>
            (_name, _raygunLoggerConfiguration) = (name, raygunLoggerConfiguration);

        //TODO: Get from https://github.com/MindscapeHQ/serilog-sinks-raygun/blob/dev/src/Serilog.Sinks.Raygun/Sinks/Raygun/RaygunSink.cs
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel) => (logLevel >= _raygunLoggerConfiguration.MinLogLevel &&
                                                     logLevel <= _raygunLoggerConfiguration.MaxLogLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            RaygunMauiClient.Current.SendInBackground(
                new Exception(formatter(state, exception)),
                _raygunLoggerConfiguration.SendDefaultTags
                    ? new List<string>() { logLevel.ToString(), Raygun4NetBuildPlatforms.GetBuildPlatform() }
                    : null,
                _raygunLoggerConfiguration.SendDefaultCustomData
                    ? new Dictionary<string, object>()
                    {
                        { "logLevel", logLevel }, { "eventId", eventId }, { "state", state }, { "name", _name },
                        { "message", formatter(state, exception) }
                    }
                    : null
            );
        }
    }
}