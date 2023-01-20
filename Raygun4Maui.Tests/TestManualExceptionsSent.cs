using Microsoft.Extensions.Configuration;
using raygun4maui;

namespace raygun4mauiUnitTesting
{
    [TestClass]
    public class TestManualExceptionsSent
    {
        private readonly String _apiKey;
        private IConfigurationRoot _configuration;

        public TestManualExceptionsSent()
        {
            _configuration = new ConfigurationBuilder()
               .AddUserSecrets<TestManualExceptionsSent>()
               .Build();

            _apiKey = _configuration["apiKey"];
        }

        [TestMethod]
        public void TestApiKeyPresent()
        {
            Assert.IsNotNull(_apiKey);
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