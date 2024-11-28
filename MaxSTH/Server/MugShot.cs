using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace STHMaxzzzie.Server
{
    public class MugShot : BaseScript
    {
        [Command("mugshot", Restricted = false)]
        async void mugShot(int source, List<object> args, string raw)
        {
            if (args.Count == 1)
            {
                int target;
                bool isArgs0Int = Int32.TryParse(args[0].ToString(), out target);
                if (isArgs0Int)
                {
                  mugShot(source, target);
                }
            }
        }

            [EventHandler("mugShot")]
            async void mugShot(int source, int target)
            {
                Debug.WriteLine("Mugshot received by server.");
            if (target != 0) 
            {
                    sendClientModelNameForOutfit(source, target);
            }
            else
            {
                //Debug.WriteLine($"test 3");
                if (RoundHandling.runnerThisGame != -1)
                {
                    sendClientModelNameForOutfit(source, RoundHandling.runnerThisGame);
                }
                else sendClientModelNameForOutfit(source, source);

            }
        }

        void sendClientModelNameForOutfit(int source, int clientIdToSendModelOf)
        {
            //Debug.WriteLine($"Running sendCLientModelNameForOutfit.");
            string name = "unknown";
            if (Playerlist.playerModels.ContainsKey(clientIdToSendModelOf))
            {
                foreach (Player player in Players)
                {

                    if (int.Parse(player.Handle) == clientIdToSendModelOf)
                    {
                        name = player.Name;
                        break;
                    }
                }
                TriggerClientEvent(Players[source], "ShowNotification", $"This is what {name} looks like now.");
                TriggerClientEvent(Players[source], "MugShotEvent", Playerlist.playerModels[clientIdToSendModelOf]);
            }
            else
            {
                TriggerClientEvent(Players[source], "ShowNotification", $"This player's skin isn't registered with the server. It's probably Michael. Or the player isn't online.");
                TriggerClientEvent(Players[source], "MugShotEvent", "player_zero");
            }
        }


    }
}