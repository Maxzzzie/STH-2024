using CitizenFX.Core;
using TwitchLib.Client.Models;
using TwitchTestClient.Server.Twitch.Commands;

namespace TwitchTestClient.Server.Twitch.Features
{
    public abstract class TwitchBaseFeature : BaseScript
    {
        internal readonly string feature;

        public TwitchBaseFeature(string feature)
        {
            this.feature = feature;
            TwitchHandler.OnChatMessageReceived += TwitchHandler_OnChatMessageReceived;
            TwitchHandler.RegisterFeature(this);
        }
 
        ~TwitchBaseFeature()
        {
            TwitchHandler.OnChatMessageReceived -= TwitchHandler_OnChatMessageReceived;
        }

        public string GetFeature() => feature;

        internal abstract void HandleFeature(ChatMessage message);
        internal virtual bool ShouldHandle(ChatMessage message)
        {
            return true;
        }

        private void TwitchHandler_OnChatMessageReceived(ChatMessage msg)
        {
            if (!TwitchHandler.IsFeatureEnabled(feature) || 
                !ShouldHandle(msg))
                return;
            HandleFeature(msg);
        }
    }
}
