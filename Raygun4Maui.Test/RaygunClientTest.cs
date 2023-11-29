using Mindscape.Raygun4Net;
using Xunit;

namespace Raygun4Maui.Test
{
    public class RaygunClientTest
    {
        [Fact]
        public void MessageDetailsTest()
        {
            var t = new RaygunMauiClient("x"); //API key must be set otherwise the message will not be created
            RaygunMessage message = null;

            t.SendingMessage += (sender, e) =>
            {
                e.Cancel = true;
                message = e.Message;
            };

            t.Send(new Exception());

            Assert.NotNull(message.Details);
        }

        [Fact]
        public void MessageDetailsEnvironmentTest()
        {
            var t = new RaygunMauiClient("x"); //API key must be set otherwise the message will not be created
            RaygunMessage message = null;

            t.SendingMessage += (sender, e) =>
            {
                e.Cancel = true;
                message = e.Message;
            };

            t.Send(new Exception());

            Assert.NotNull(message.Details.Environment);

        }

        [Fact]
        public void MessageDetailsClientTest()
        {
            var t = new RaygunMauiClient("x"); //API key must be set otherwise the message will not be created
            RaygunMessage message = null;

            t.SendingMessage += (sender, e) =>
            {
                e.Cancel = true;
                message = e.Message;

            };

            t.Send(new Exception());
            Assert.NotNull(message.Details.Client);
        }
    }
}