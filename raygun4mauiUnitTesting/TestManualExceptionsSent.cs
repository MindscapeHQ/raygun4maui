using raygun4maui;
using System.Runtime.ConstrainedExecution;

namespace raygun4mauiUnitTesting
{
    [TestClass]
    public class TestManualExceptionsSent
    {
        private readonly String _apiKey;

        [TestMethod]
        public void TestSend()
        {
            RaygunMauiClient raygunMauiClient = new RaygunMauiClient(_apiKey);
        }
    }
}