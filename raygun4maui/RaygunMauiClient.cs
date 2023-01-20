using Mindscape.Raygun4Net;

namespace raygun4maui
{
    // All the code in this file is included in all platforms.
    public class RaygunMauiClient
    {
        private readonly RaygunClient _raygunClient;
        public RaygunMauiClient(String apiKey)
        {
            _raygunClient = new RaygunClient(apiKey);
        }

        public void Send(Exception exception)
        {
            _raygunClient.Send(exception);
        }

        public Task SendInBackground(Exception exception) 
        {
            return _raygunClient.SendInBackground(exception);
        }
    }
}