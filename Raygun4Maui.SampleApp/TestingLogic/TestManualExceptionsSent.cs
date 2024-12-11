using Mindscape.Raygun4Net;

namespace Raygun4Maui.SampleApp.TestingLogic
{
    internal class TestManualExceptionsSent
    {
        IRaygunMauiUserProvider _userProvider;
        Raygun4MauiSettings _settings;
        
        public TestManualExceptionsSent(IRaygunMauiUserProvider userProvider, Raygun4MauiSettings settings)
        {
            _userProvider = userProvider;
            _settings = settings;
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

        private async void TestSend()
        {
            await RaygunMauiClient.Current.SendAsync(GenerateException("TestSend"));
        }

        private void TestSendInBackground()
        {
            RaygunMauiClient.Current.SendInBackground(GenerateException("TestSendInBackground")).Wait();
        }

        private async void TestCustomGrouping()
        {
            RaygunMauiClient.Current.CustomGroupingKey += RaygunClient_CustomGroupingKey;

            await RaygunMauiClient.Current.SendAsync(GenerateException("TestCustomGrouping"));
            await RaygunMauiClient.Current.SendAsync(GenerateException("TestCustomGrouping"));
        }
        private static void RaygunClient_CustomGroupingKey(object sender, RaygunCustomGroupingKeyEventArgs e)
        {
            e.CustomGroupingKey = "TestCustomGrouping";
        }

        private async void TestUniqueUserTracking()
        {
            _userProvider.SetUser(new RaygunIdentifierMessage("user1@email.com"));
            
            await RaygunMauiClient.Current.SendAsync(GenerateException("TestUniqueUserTracking"));

            _userProvider.SetUser(new RaygunIdentifierMessage("user2@email.com")
            {
                IsAnonymous = false,
                FullName = "Robbie Robot",
                FirstName = "Robbie"
            });
            
            await RaygunMauiClient.Current.SendAsync(GenerateException("TestUniqueUserTracking"));

            await RaygunMauiClient.Current.SendInBackground(GenerateException("TestUniqueUserTracking"), null, null, new RaygunIdentifierMessage("user3@email.com") { IsAnonymous = false, FullName = "Robbie Robot", FirstName = "Robbie" });
        }

        private async void TestTags()
        {
            await RaygunMauiClient.Current.SendAsync(GenerateException("TestTags"), new List<string>() { "tag1", "tag2" });
        }

        private async void TestCustomData()
        {
            await RaygunMauiClient.Current.SendAsync(GenerateException("TestCustomData"), null, new Dictionary<string, object>() { { "key", "value" } });
        }

        private void TestVersionNumbering()
        {
            _settings.RaygunSettings.ApplicationVersion = "TestVersionNumbering";
            RaygunMauiClient.Current.SendAsync(GenerateException("TestVersionNumbering"));
        }
    }
}
