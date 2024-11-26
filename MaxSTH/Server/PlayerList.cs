using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace STHMaxzzzie.Server
{
    public class Playerlist : BaseScript
    {
        public static Dictionary<int, string> playerModels = new Dictionary<int, string>();

        [EventHandler("updateServerModel")]
        void UpdateServerModel(int source, string model)
        {
            //TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"updated player model serverside. {source} {model} {playerModels.Count}" } });
            playerModels[source] = model;
            Debug.WriteLine($"dict with {source} and {model}");
        }

        [Command("players", Restricted = false)]
        [EventHandler("playerList")]
        void playerList(int source, List<object> args, string raw)
        {
            //TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"All players in the playerlist." } });
            foreach (Player player in Players)
            {
                int playerId = int.Parse(player.Handle);
                if (playerModels.ContainsKey(playerId))
                {
                    //TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"{player.Name}({player.Handle}) Model:\"{playerModels[playerId]}\" Ping:{player.Ping}" } });
                    TriggerClientEvent(Players[source], "ShowNotification", $"{player.Name}({player.Handle}) Model:\"{playerModels[playerId]}\" Ping:{player.Ping}");
                    //TriggerClientEvent(Players[source], "showDebugMessage", $"{player.Name}({player.Handle}) Model: \"{playerModels[playerId]}\" Ping:{player.Ping}");
                    
                }
                else
                {
                    //TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 153, 153 }, args = new[] { $"{player.Name}({player.Handle}) Model:default Ping:{player.Ping}" } });
                   TriggerClientEvent(Players[source], "ShowNotification",  $"{player.Name}({player.Handle}) Model:default Ping:{player.Ping}");
                    //TriggerClientEvent(Players[source], "showDebugMessage", $"{player.Name}({player.Handle}) Model:default Ping:{player.Ping}");
                }
            }
        }
    }
}

