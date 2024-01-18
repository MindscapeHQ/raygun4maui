using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

// Namespace is different, doesn't include Raygun4Maui because we may want to move this to Raygun4Net
// ReSharper disable once CheckNamespace
namespace Raygun4Net.RaygunLogger
{
    public sealed class RaygunLoggerProvider : ILoggerProvider
    {
        private RaygunLoggerConfiguration _raygunLoggerConfiguration;
        private readonly ConcurrentDictionary<string, RaygunLogger> _loggers = new(StringComparer.OrdinalIgnoreCase);

        public RaygunLoggerProvider(RaygunLoggerConfiguration raygunLoggerConfiguration)
        {
            _raygunLoggerConfiguration = raygunLoggerConfiguration;
        }

        public ILogger CreateLogger(string categoryName) =>
            _loggers.GetOrAdd(categoryName, name => new RaygunLogger(name, _raygunLoggerConfiguration));

        public void Dispose()
        {
            _loggers.Clear();
        }
    }
}
