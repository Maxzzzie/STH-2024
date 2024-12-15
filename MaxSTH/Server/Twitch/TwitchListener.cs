using System;
using TwitchLib.Client.Models;
using TwitchLib.Client;
using System.Diagnostics;
using CitizenFX.Core;

namespace Twitch
{
    public class TwitchListener
    {
        public static event Action<ChatMessage> OnMessageReceived;

        private readonly TwitchClient client;
        private readonly string channel;

        public TwitchListener(string username, string channel, string accessToken)
        {
            this.channel = channel;

            var credentails = new ConnectionCredentials(username, accessToken);
            client = new TwitchClient();

            client.OnMessageReceived += Client_OnMessageReceived;
            client.Initialize(credentails, channel);
        }

        ~TwitchListener()
        {
            client.Disconnect();
            client.OnMessageReceived -= Client_OnMessageReceived;
        }

        public void Start()
        {
            client.Connect();
        }

        public void Stop()
        {
            client.Disconnect();
        }

        private void Client_OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
        {
            OnMessageReceived?.Invoke(e.ChatMessage);
        }
    }
}
