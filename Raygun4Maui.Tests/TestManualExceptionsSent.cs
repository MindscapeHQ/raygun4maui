using Microsoft.Extensions.Configuration;
using Mindscape.Raygun4Maui;
using Mindscape.Raygun4Net;

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

        [TestMethod]
        public void TestCustomGrouping()
        {
            RaygunMauiClient raygunMauiClient = new RaygunMauiClient(_apiKey);
            raygunMauiClient.CustomGroupingKey += _raygunClient_CustomGroupingKey;

            raygunMauiClient.Send(new Exception("raygun4mauiUnitTesting: TestCustomGrouping " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));
            raygunMauiClient.Send(new Exception("raygun4mauiUnitTesting: TestCustomGrouping " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));
        }
        private void _raygunClient_CustomGroupingKey(object? sender, RaygunCustomGroupingKeyEventArgs e)
        {
            e.CustomGroupingKey = "TestCustomGrouping";
        }

        [TestMethod]
        public void TestUniqueUserTracking()
        {
            RaygunMauiClient raygunMauiClient = new RaygunMauiClient(_apiKey);
            raygunMauiClient.User = "user@email.com";
            raygunMauiClient.Send(new Exception("raygun4mauiUnitTesting: TestUniqueUserTracking " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));

            raygunMauiClient.UserInfo = new RaygunIdentifierMessage("user@email.com")
            {
                IsAnonymous = false,
                FullName = "Robbie Robot",
                FirstName = "Robbie"
            };
            raygunMauiClient.Send(new Exception("raygun4mauiUnitTesting: TestUniqueUserTracking " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));

            raygunMauiClient.SendInBackground(new Exception("raygun4mauiUnitTesting: TestUniqueUserTracking " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")), null, null, new RaygunIdentifierMessage("user@email.com") { IsAnonymous = false, FullName = "Robbie Robot", FirstName = "Robbie" });
        }
    }
}