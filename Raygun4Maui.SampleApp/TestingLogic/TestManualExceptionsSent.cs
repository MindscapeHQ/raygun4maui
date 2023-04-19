using Mindscape.Raygun4Net;

namespace Raygun4Maui.SampleApp.TestingLogic
{
    internal class TestManualExceptionsSent
    {
        private readonly string _apiKey;
        public TestManualExceptionsSent(string apiKey)
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

            RaygunMauiClient.Current.Send(GenerateException("TestSend"));
        }

        private void TestSendInBackground()
        {

            RaygunMauiClient.Current.SendInBackground(GenerateException("TestSendInBackground")).Wait();
        }

        private void TestCustomGrouping()
        {

            RaygunMauiClient.Current.CustomGroupingKey += RaygunClient_CustomGroupingKey;

            RaygunMauiClient.Current.Send(GenerateException("TestCustomGrouping"));
            RaygunMauiClient.Current.Send(GenerateException("TestCustomGrouping"));
        }
        private static void RaygunClient_CustomGroupingKey(object sender, RaygunCustomGroupingKeyEventArgs e)
        {
            e.CustomGroupingKey = "TestCustomGrouping";
        }

        private void TestUniqueUserTracking()
        {

            RaygunMauiClient.Current.User = "user1@email.com";
            RaygunMauiClient.Current.Send(GenerateException("TestUniqueUserTracking"));

            RaygunMauiClient.Current.UserInfo = new RaygunIdentifierMessage("user2@email.com")
            {
                IsAnonymous = false,
                FullName = "Robbie Robot",
                FirstName = "Robbie"
            };
            RaygunMauiClient.Current.Send(GenerateException("TestUniqueUserTracking"));

            RaygunMauiClient.Current.SendInBackground(GenerateException("TestUniqueUserTracking"), null, null, new RaygunIdentifierMessage("user3@email.com") { IsAnonymous = false, FullName = "Robbie Robot", FirstName = "Robbie" });
        }

        private void TestTags()
        {
            RaygunMauiClient.Current.Send(GenerateException("TestTags"), new List<string>() { "tag1", "tag2" });
        }

        private void TestCustomData()
        {
            RaygunMauiClient.Current.Send(GenerateException("TestCustomData"), null, new Dictionary<string, object>() { { "key", "value" } });
        }

        private void TestVersionNumbering()
        {
            RaygunMauiClient.Current.ApplicationVersion = "TestVersionNumbering";
            RaygunMauiClient.Current.Send(GenerateException("TestVersionNumbering"));
        }
    }
}
