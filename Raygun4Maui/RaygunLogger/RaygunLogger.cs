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

        public bool IsEnabled(LogLevel logLevel) =>
            _getCurrentConfig().LogLevelToRaygunSettings.ContainsKey(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            RaygunLoggerConfiguration raygunLoggerConfiguration = _getCurrentConfig();

            RaygunClient raygunClient = RaygunClientFactory(_getCurrentConfig().LogLevelToRaygunSettings[logLevel]);
            raygunClient.SendInBackground(
                exception,
                _getCurrentConfig().LogLevelToRaygunSettings[logLevel].SendDefaultTags ? new List<string>() { _name, logLevel.ToString()} : null,
                _getCurrentConfig().LogLevelToRaygunSettings[logLevel].SendDefaultCustomData ? new Dictionary<string, object>() { {"logLevel", logLevel}, {"eventId", eventId}, { "state", state }, { "name", _name }, {"message", formatter(state, exception) } } : null
            );
        }

        private static RaygunClient RaygunClientFactory(RaygunSettingsBase raygunSettingsBase) =>
            new(raygunSettingsBase);

    }
}
