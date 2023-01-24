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

        public void RunAllTests()
        {
            TestSend();
            TestSendInBackground();
            TestCustomGrouping();
            TestUniqueUserTracking();
            TestTags();
            TestCustomData();
            TestVersionNumbering();
        }

        private static Exception GenerateException(string methodName)
        {
            return new Exception("Raygun4Maui.SampleApp.TestManualExceptionsSent: " + methodName + " @ " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private void TestSend()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);
            raygunMauiClient.Send(GenerateException("TestSend"));
        }

        private void TestSendInBackground()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);
            raygunMauiClient.SendInBackground(GenerateException("TestSendInBackground")).Wait();
        }

        private void TestCustomGrouping()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);
            raygunMauiClient.CustomGroupingKey += RaygunClient_CustomGroupingKey;

            raygunMauiClient.Send(GenerateException("TestCustomGrouping"));
            raygunMauiClient.Send(GenerateException("TestCustomGrouping"));
        }
        private static void RaygunClient_CustomGroupingKey(object sender, RaygunCustomGroupingKeyEventArgs e)
        {
            e.CustomGroupingKey = "TestCustomGrouping";
        }

        private void TestUniqueUserTracking()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);
            raygunMauiClient.User = "user1@email.com";
            raygunMauiClient.Send(GenerateException("TestUniqueUserTracking"));

            raygunMauiClient.UserInfo = new RaygunIdentifierMessage("user2@email.com")
            {
                IsAnonymous = false,
                FullName = "Robbie Robot",
                FirstName = "Robbie"
            };
            raygunMauiClient.Send(GenerateException("TestUniqueUserTracking"));

            raygunMauiClient.SendInBackground(GenerateException("TestUniqueUserTracking"), null, null, new RaygunIdentifierMessage("user3@email.com") { IsAnonymous = false, FullName = "Robbie Robot", FirstName = "Robbie" });
        }

        private void TestTags()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);

            raygunMauiClient.Send(GenerateException("TestTags"), new List<string>() { "tag1", "tag2" });
        }

        private void TestCustomData()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);

            raygunMauiClient.Send(GenerateException("TestCustomData"), null, new Dictionary<string, object>() { { "key", "value" } });
        }

        private void TestVersionNumbering()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);
            raygunMauiClient.ApplicationVersion = "TestVersionNumbering";
            raygunMauiClient.Send(GenerateException("TestVersionNumbering"));
        }
    }
}
