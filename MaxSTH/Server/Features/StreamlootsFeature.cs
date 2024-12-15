using System.Collections.Generic;
using TwitchLib.Client.Models;
using TwitchTestClient.Server.Twitch.Commands;
using TwitchTestClient.Server.Twitch.Features;

namespace TwitchTestClient.Server.Features
{
    public class StreamlootsFeature : TwitchBaseFeature
    {
        public Dictionary<string, string> textToCommand = new Dictionary<string , string>();
        //private const string startsWith = "[streamloots]";

        public StreamlootsFeature() : base("streamloots")
        {
            textToCommand.Add("Whoah, kickflip!", "launch");
        }
        internal override bool ShouldHandle(ChatMessage message)
        {

            if (message.DisplayName.ToLower() != "itsjustgilly" || message.DisplayName.ToLower() != "maxzzzie")
            {
                return false;
            }
            //return message.Message.StartsWith(startsWith);
            return true;
            
        }
        internal override void HandleFeature(ChatMessage message)
        {
            //string action = message.Message.Substring(startsWith.Length).Trim();
            //TwitchHandler.Instance.TriggerMessage($"STREAMLOOTS TRIGGERED");
            if (textToCommand.ContainsKey(message.Message))
            {
                TriggerClientEvent("StreamLootsEffect", textToCommand[message.Message]);
            }

                        
        }
    }
}
