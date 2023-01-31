using Mindscape.Raygun4Maui;

namespace Raygun4Maui.SampleApp.TestingLogic
{
    internal class TestUnhandledExceptionsSent
    {
        public static void RunAllTests()
        {
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
