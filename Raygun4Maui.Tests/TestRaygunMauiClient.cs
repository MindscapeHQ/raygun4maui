using Mindscape.Raygun4Maui;
using Raygun4Maui.MattJohnsonPint.Maui;

namespace Raygun4Maui.Tests
{
    [TestClass]
    public class TestRaygunMauiClient
    {
        [TestMethod]
        public void TestRaygunMauiClientCreated()
        {
            RaygunMauiClient raygunMauiClient = new("");

            Assert.IsNotNull(raygunMauiClient);
        }
    }
}
