using Mindscape.Raygun4Maui;
using Mindscape.Raygun4Net;

namespace Raygun4Maui.SampleApp
{
    internal class TestManualExceptionsSent
    {
        private readonly string _apiKey;
        public TestManualExceptionsSent(String apiKey)
        {
            _apiKey = apiKey;
        }

        public void runAllTests()
        {
            TestSend();
            TestSendInBackground();
            TestCustomGrouping();
            TestUniqueUserTracking();
        }

        private void TestSend()
        {
            RaygunMauiClient raygunMauiClient = new RaygunMauiClient(_apiKey);
            raygunMauiClient.Send(new Exception("Raygun4Maui.SampleApp: TestSend " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));
        }

        private void TestSendInBackground()
        {
            RaygunMauiClient raygunMauiClient = new RaygunMauiClient(_apiKey);
            raygunMauiClient.SendInBackground(new Exception("Raygun4Maui.SampleApp: SendInBackground " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))).Wait();
        }

        private void TestCustomGrouping()
        {
            RaygunMauiClient raygunMauiClient = new RaygunMauiClient(_apiKey);
            raygunMauiClient.CustomGroupingKey += _raygunClient_CustomGroupingKey;

            raygunMauiClient.Send(new Exception("Raygun4Maui.SampleApp: TestCustomGrouping " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));
            raygunMauiClient.Send(new Exception("Raygun4Maui.SampleApp: TestCustomGrouping " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));
        }
        private void _raygunClient_CustomGroupingKey(object sender, RaygunCustomGroupingKeyEventArgs e)
        {
            e.CustomGroupingKey = "TestCustomGrouping";
        }

        private void TestUniqueUserTracking()
        {
            RaygunMauiClient raygunMauiClient = new RaygunMauiClient(_apiKey);
            raygunMauiClient.User = "user1@email.com";
            raygunMauiClient.Send(new Exception("Raygun4Maui.SampleApp: TestUniqueUserTracking " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));

            raygunMauiClient.UserInfo = new RaygunIdentifierMessage("user2@email.com")
            {
                IsAnonymous = false,
                FullName = "Robbie Robot",
                FirstName = "Robbie"
            };
            raygunMauiClient.Send(new Exception("Raygun4Maui.SampleApp: TestUniqueUserTracking " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")));

            raygunMauiClient.SendInBackground(new Exception("Raygun4Maui.SampleApp: TestUniqueUserTracking " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")), null, null, new RaygunIdentifierMessage("user3@email.com") { IsAnonymous = false, FullName = "Robbie Robot", FirstName = "Robbie" });
        }
    }
}
