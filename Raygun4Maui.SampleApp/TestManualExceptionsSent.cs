﻿using Mindscape.Raygun4Maui;
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
            TestTags();
            TestCustomData();
            TestVersionNumbering();
        }

        private Exception _generateException(string methodName)
        {
            return new Exception("Raygun4Maui.SampleApp: " + methodName + " @ " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private void TestSend()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);
            raygunMauiClient.Send(_generateException("TestSend"));
        }

        private void TestSendInBackground()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);
            raygunMauiClient.SendInBackground(_generateException("TestSendInBackground")).Wait();
        }

        private void TestCustomGrouping()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);
            raygunMauiClient.CustomGroupingKey += _raygunClient_CustomGroupingKey;

            raygunMauiClient.Send(_generateException("TestCustomGrouping"));
            raygunMauiClient.Send(_generateException("TestCustomGrouping"));
        }
        private void _raygunClient_CustomGroupingKey(object sender, RaygunCustomGroupingKeyEventArgs e)
        {
            e.CustomGroupingKey = "TestCustomGrouping";
        }

        private void TestUniqueUserTracking()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);
            raygunMauiClient.User = "user1@email.com";
            raygunMauiClient.Send(_generateException("TestUniqueUserTracking"));

            raygunMauiClient.UserInfo = new RaygunIdentifierMessage("user2@email.com")
            {
                IsAnonymous = false,
                FullName = "Robbie Robot",
                FirstName = "Robbie"
            };
            raygunMauiClient.Send(_generateException("TestUniqueUserTracking"));

            raygunMauiClient.SendInBackground(_generateException("TestUniqueUserTracking"), null, null, new RaygunIdentifierMessage("user3@email.com") { IsAnonymous = false, FullName = "Robbie Robot", FirstName = "Robbie" });
        }

        private void TestTags()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);

            raygunMauiClient.Send(_generateException("TestTags"), new List<string>() { "tag1", "tag2" });
        }

        private void TestCustomData()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);

            raygunMauiClient.Send(_generateException("TestCustomData"), null, new Dictionary<string, object>() { { "key", "value" } });
        }

        private void TestVersionNumbering()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);
            raygunMauiClient.ApplicationVersion = "TestVersionNumbering";
            raygunMauiClient.Send(_generateException("TestVersionNumbering"));
        }
    }
}