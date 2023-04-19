using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using Raygun4Maui;


namespace Raygun4Maui.SampleApp.TestingLogic
{
    internal class TestLoggerErrorsSent
    {
        private readonly ILogger _logger;

        public TestLoggerErrorsSent(string apiKey, ILogger logger)
        {
            _logger = logger;
        }

        public void RunAllTests()
        {
            TestLogTrace();
            TestLogDebug();
            TestLogInformation();
            TestLogWarning();
            TestLogError();
            TestLogCritical();
            TestLogException();
        }

        private void TestLogTrace()
        {
            //CA2254 (incorrectly?) prevents exporting all templates into a string field
            _logger.LogTrace("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogTrace", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private void TestLogDebug()
        {
            _logger.LogDebug("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogDebug", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        private void TestLogInformation()
        {
            _logger.LogInformation("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogInformation", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        private void TestLogWarning()
        {
            _logger.LogWarning("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogWarning", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        private void TestLogError()
        {
            _logger.LogError("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogError", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        private void TestLogCritical()
        {
            _logger.LogCritical("Raygun4Maui.SampleApp.TestLoggerErrorsSent: {MethodName} @ {Timestamp}", "TestLogCritical", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        private void TestLogException()
        {
            try
            {
                TestLofExceptionInnerMethod();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e, "TestLogException exception caught at {Timestamp}", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            }

        }

        private static void TestLofExceptionInnerMethod()
        {
            throw new Exception("Raygun4Maui.SampleApp.TestLofExceptionInnerMethod");
        }
    }
}