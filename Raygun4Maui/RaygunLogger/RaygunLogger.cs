using Microsoft.Extensions.Logging;
using Mindscape.Raygun4Net;

namespace Raygun4Maui.RaygunLogger
{
    public sealed class RaygunLogger : ILogger
    {
        private readonly string _name;
        private readonly Func<RaygunLoggerConfiguration> _getCurrentConfig;

        public RaygunLogger(string name, Func<RaygunLoggerConfiguration> getCurrentConfig) =>
            (_name, _getCurrentConfig) = (name, getCurrentConfig);

        //TODO: Get from https://github.com/MindscapeHQ/serilog-sinks-raygun/blob/dev/src/Serilog.Sinks.Raygun/Sinks/Raygun/RaygunSink.cs
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel) => (logLevel >= _getCurrentConfig().MinLogLevel && logLevel <= _getCurrentConfig().MaxLogLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            RaygunClient raygunClient = RaygunClientFactory(_getCurrentConfig());
            raygunClient.SendInBackground(
                new Exception(formatter(state, exception)),
                _getCurrentConfig().SendDefaultTags ? new List<string>() {logLevel.ToString()} : null,
                _getCurrentConfig().SendDefaultCustomData ? new Dictionary<string, object>() { {"logLevel", logLevel}, {"eventId", eventId}, { "state", state }, { "name", _name }, {"message", formatter(state, exception) } } : null
            );
        }

        private static RaygunClient RaygunClientFactory(RaygunSettingsBase raygunSettingsBase) =>
            new(raygunSettingsBase);

    }
}
