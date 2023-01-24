using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Raygun4Maui.RaygunLogger
{
    [ProviderAlias("Raygun")]
    public sealed class RaygunLoggerProvider : ILoggerProvider
    {
        private readonly IDisposable _onChangeToken;
        private RaygunLoggerConfiguration _currentConfig;
        private readonly ConcurrentDictionary<string, RaygunLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

        public RaygunLoggerProvider(IOptionsMonitor<RaygunLoggerConfiguration> config)
        {
            _currentConfig = config.CurrentValue;
            _onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
        }

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new RaygunLogger(name, GetCurrentConfig));

        private RaygunLoggerConfiguration GetCurrentConfig() => _currentConfig;

        public void Dispose()
        {
            _loggers.Clear();
            _onChangeToken?.Dispose();
        }
    }
}
