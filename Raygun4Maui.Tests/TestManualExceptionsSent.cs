using Microsoft.Extensions.Configuration;
using Mindscape.Raygun4Maui;

namespace Raygun4Maui.Tests
{
    [TestClass]
    public class TestManualExceptionsSent
    {
        private readonly String _apiKey;

        public TestManualExceptionsSent()
        {
            var configuration = new ConfigurationBuilder()
               .AddUserSecrets<TestManualExceptionsSent>()
               .Build();

            _apiKey = configuration["apiKey"] ?? "";            
        }

        [TestMethod]
        public void TestApiKeyPresent()
        {
            Assert.AreNotEqual("", _apiKey);
        }

        [TestMethod]
        public void TestSend()
        {
            RaygunMauiClient raygunMauiClient = new RaygunMauiClient(_apiKey);
            raygunMauiClient.Send(new Exception("raygun4mauiUnitTesting: TestSend " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));
        }

        [TestMethod]
        public void TestSendInBackground()
        {
            RaygunMauiClient raygunMauiClient = new RaygunMauiClient(_apiKey);
            raygunMauiClient.SendInBackground(new Exception("raygun4mauiUnitTesting: SendInBackground " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))).Wait();
        }
    }
}