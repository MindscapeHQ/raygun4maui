using Microsoft.Extensions.Logging;
using Mindscape.Raygun4Net;

namespace Raygun4Maui.RaygunLogger
{
    public abstract class AbstractRaygunLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            RaygunClient raygunClient = RaygunClientFactory();
            raygunClient.SendInBackground(exception);
        }

        protected abstract RaygunClient RaygunClientFactory();
    }
}
