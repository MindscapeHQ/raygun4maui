using Mindscape.Raygun4Maui;

namespace Raygun4Maui.SampleApp
{
    internal class TestUnhandledExceptionsSent
    {
        private readonly string _apiKey;
        public TestUnhandledExceptionsSent(String apiKey)
        {
            _apiKey = apiKey;
        }

        public void RunAllTests()
        {
            RaygunMauiClient raygunMauiClient = new(_apiKey);
            TestThrowUnhandledException();
        }

        private static Exception GenerateException(string methodName)
        {
            return new Exception("Raygun4Maui.SampleApp.TestUnhandledExceptionsSent: " + methodName + " @ " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private static void TestThrowUnhandledException()
        {
            throw GenerateException("TestThrowUnhandledException");
        }
    }
}
