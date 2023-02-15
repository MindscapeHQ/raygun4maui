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
            RaygunClient raygunClient = new(_apiKey);
            raygunClient.Send(GenerateException("TestSend"));
        }

        private void TestSendInBackground()
        {
            RaygunClient raygunClient = new(_apiKey);
            raygunClient.SendInBackground(GenerateException("TestSendInBackground")).Wait();
        }

        private void TestCustomGrouping()
        {
            RaygunClient raygunClient = new(_apiKey);
            raygunClient.CustomGroupingKey += RaygunClient_CustomGroupingKey;

            raygunClient.Send(GenerateException("TestCustomGrouping"));
            raygunClient.Send(GenerateException("TestCustomGrouping"));
        }
        private static void RaygunClient_CustomGroupingKey(object sender, RaygunCustomGroupingKeyEventArgs e)
        {
            e.CustomGroupingKey = "TestCustomGrouping";
        }

        private void TestUniqueUserTracking()
        {
            RaygunClient raygunClient = new(_apiKey);
            raygunClient.User = "user1@email.com";
            raygunClient.Send(GenerateException("TestUniqueUserTracking"));

            raygunClient.UserInfo = new RaygunIdentifierMessage("user2@email.com")
            {
                IsAnonymous = false,
                FullName = "Robbie Robot",
                FirstName = "Robbie"
            };
            raygunClient.Send(GenerateException("TestUniqueUserTracking"));

            raygunClient.SendInBackground(GenerateException("TestUniqueUserTracking"), null, null, new RaygunIdentifierMessage("user3@email.com") { IsAnonymous = false, FullName = "Robbie Robot", FirstName = "Robbie" });
        }

        private void TestTags()
        {
            RaygunClient raygunClient = new(_apiKey);

            raygunClient.Send(GenerateException("TestTags"), new List<string>() { "tag1", "tag2" });
        }

        private void TestCustomData()
        {
            RaygunClient raygunClient = new(_apiKey);

            raygunClient.Send(GenerateException("TestCustomData"), null, new Dictionary<string, object>() { { "key", "value" } });
        }

        private void TestVersionNumbering()
        {
            RaygunClient raygunClient = new(_apiKey);
            raygunClient.ApplicationVersion = "TestVersionNumbering";
            raygunClient.Send(GenerateException("TestVersionNumbering"));
        }
    }
}
