using System.Collections.Generic;
using TwitchLib.Client.Models;
using TwitchTestClient.Server.Twitch.Commands;
using TwitchTestClient.Server.Twitch.Features;
using STHMaxzzzie.Server;
using CitizenFX.Core;
using CitizenFX;
using CitizenFX.Core.Native;

namespace TwitchTestClient.Server.Features
{
    public class StreamlootsFeature : TwitchBaseFeature
    {
        static public Dictionary<string, string> textToCommand = new Dictionary<string, string>();
        //private const string startsWith = "[streamloots]";

        public StreamlootsFeature() : base("streamloots")
        {
            textToCommand = LoadResources.streamLootsCardInfo();
        }
        internal override bool ShouldHandle(ChatMessage message)
        {

            if (message.DisplayName.ToLower() != "itsjustgilly" && message.DisplayName.ToLower() != "maxzzzie" )
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
            Debug.WriteLine("Twitch handle feature");
            if (textToCommand.ContainsKey(message.Message))
            {
                string line = textToCommand[message.Message];
                string[] parts = line.Split('*');
                if (RoundHandling.gameMode != "none")
                {
                    int target = int.Parse(parts[3]);
                    int runnerId = RoundHandling.targetThisGame;
                    if (target == -1) //send effect to all players
                    {
                        TriggerClientEvent("StreamLootsEffect", parts[0]);
                    }
                    else if (target == 1) //runner
                    {
                        TriggerClientEvent(Players[runnerId], "StreamLootsEffect", parts[0]);
                    }
                    else if (target == 2) //hunters
                    {

                        foreach (Player player in Players)
                        {
                            int playerId = int.Parse(player.Handle.ToString());
                            if (playerId != runnerId)
                            {
                                TriggerClientEvent(player, "StreamLootsEffect", parts[0]);
                            }
                        }
                    }
                }
                else
                {
                    Debug.WriteLine("A StreamLoots command came through but there is no round started.");
                    //TriggerClientEvent(player, "ShowNotification", text");
                    TriggerClientEvent("chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"Streamloots effect not triggered because there is no round active."}});                  
                   }
            }
        }
    }
}
