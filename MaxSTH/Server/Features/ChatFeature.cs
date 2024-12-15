using TwitchLib.Client.Models;
using TwitchTestClient.Server.Twitch.Commands;
using TwitchTestClient.Server.Twitch.Features;

namespace TwitchTestClient.Server.Features
{
    public class ChatFeature : TwitchBaseFeature
    {
        public ChatFeature() : base("chat")
        {

        }

        internal override bool ShouldHandle(ChatMessage message)
        {
            // Assume other features start with '['
            return !message.Message.StartsWith("[");
        }

        internal override void HandleFeature(ChatMessage message)
        {
            TwitchHandler.Instance.TriggerMessage($"{message.DisplayName}: {message.Message}");

            
        }
    }
}
