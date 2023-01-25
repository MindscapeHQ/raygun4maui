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

        public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            RaygunLoggerConfiguration raygunLoggerConfiguration = _getCurrentConfig();
            RaygunLoggerSettings raygunLoggerSettings = _getCurrentConfig().RaygunLoggerSettings;

            RaygunClient raygunClient = RaygunClientFactory(raygunLoggerSettings);
            raygunClient.SendInBackground(
                exception,
                raygunLoggerSettings.SendDefaultTags ? new List<string>() { _name, logLevel.ToString()} : null,
                raygunLoggerSettings.SendDefaultCustomData ? new Dictionary<string, object>() { {"logLevel", logLevel}, {"eventId", eventId}, { "state", state }, { "name", _name }, {"message", formatter(state, exception) } } : null
            );
        }

        private static RaygunClient RaygunClientFactory(RaygunSettingsBase raygunSettingsBase) =>
            new(raygunSettingsBase);

    }
}
