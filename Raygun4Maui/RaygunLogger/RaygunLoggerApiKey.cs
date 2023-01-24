using Mindscape.Raygun4Net;

namespace Raygun4Maui.RaygunLogger
{
    public sealed class RaygunLoggerApiKey : AbstractRaygunLogger
    {
        private readonly string _apiKey;

        public RaygunLoggerApiKey(string apiKey)
        {
            _apiKey = apiKey;
        }   

        protected override RaygunClient RaygunClientFactory()
        {
            return new RaygunClient(_apiKey);
        }
    }
}
