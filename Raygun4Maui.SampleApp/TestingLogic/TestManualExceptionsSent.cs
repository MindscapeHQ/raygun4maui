using Mindscape.Raygun4Net;

namespace Raygun4Maui.SampleApp.TestingLogic
{
    internal class TestManualExceptionsSent
    {
        private readonly IRaygunMauiUserProvider _userProvider;

        public TestManualExceptionsSent(IRaygunMauiUserProvider userProvider)
        {
            _userProvider = userProvider;
        }

        public async Task RunAllTests()
        {
            await TestSendAsync();
            await TestSendInBackground();
            await TestCustomGrouping();
            await TestUniqueUserTracking();
            await TestTags();
            await TestCustomData();
        }

        private static Exception GenerateException(string methodName)
        {
            return new Exception("Raygun4Maui.SampleApp.TestManualExceptionsSent: " + methodName + " @ " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private async Task TestSendAsync()
        {
            await RaygunMauiClient.Current.SendAsync(GenerateException("TestSend"));
        }

        private async Task TestSendInBackground()
        {
            await RaygunMauiClient.Current.SendInBackground(GenerateException("TestSendInBackground"));
        }

        private async Task TestCustomGrouping()
        {
            RaygunMauiClient.Current.CustomGroupingKey += RaygunClient_CustomGroupingKey;

            await RaygunMauiClient.Current.SendAsync(GenerateException("TestCustomGrouping"));
            await RaygunMauiClient.Current.SendAsync(GenerateException("TestCustomGrouping"));
        }
        private static void RaygunClient_CustomGroupingKey(object sender, RaygunCustomGroupingKeyEventArgs e)
        {
            e.CustomGroupingKey = "TestCustomGrouping";
        }

        private async Task TestUniqueUserTracking()
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
            
            _userProvider.SetUser(new RaygunIdentifierMessage(null) { IsAnonymous = true });
        }

        private async Task TestTags()
        {
            await RaygunMauiClient.Current.SendAsync(GenerateException("TestTags"), new List<string>() { "tag1", "tag2" });
        }

        private async Task TestCustomData()
        {
            await RaygunMauiClient.Current.SendAsync(GenerateException("TestCustomData"), null, new Dictionary<string, object>() { { "key", "value" } });
        }
    }
}
