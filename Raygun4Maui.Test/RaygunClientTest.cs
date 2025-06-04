using Mindscape.Raygun4Net;
using Xunit;

namespace Raygun4Maui.Test
{
    public class RaygunClientTest(SendingMessageTestFixture fixture) : IClassFixture<SendingMessageTestFixture>
    {
        [Fact]
        public async Task MessageDetailsTest()
        {
            await RaygunMauiClient.Current.SendAsync(new Exception());
            Assert.NotNull(fixture.Message.Details);
        }

        [Fact]
        public async Task MessageDetailsEnvironmentTest()
        {
            await RaygunMauiClient.Current.SendAsync(new Exception());
            Assert.NotNull(fixture.Message.Details.Environment);
        }

        [Fact]
        public async Task MessageDetailsClientTest()
        {
            await RaygunMauiClient.Current.SendAsync(new Exception());
            Assert.NotNull(fixture.Message.Details.Client);
        }
    }

    public class SendingMessageTestFixture : IDisposable
    {
        public RaygunMessage Message { get; set; }
        
        public SendingMessageTestFixture()
        {
            RaygunMauiClient.Current.SendingMessage += OnSendingMessage;
        }

        private void OnSendingMessage(object sender, RaygunSendingMessageEventArgs e)
        {
            e.Cancel = true;
            Message = e.Message;
        }

        public void Dispose() => RaygunMauiClient.Current.SendingMessage -= OnSendingMessage;
    }
}