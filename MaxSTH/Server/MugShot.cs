using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace STHMaxzzzie.Server
{
    public class MugShots : BaseScript
    {
        [Command("mugshot", Restricted = false)]
        void MugShot(int source, List<object> args, string raw)
        {
            if (args.Count == 1)
            {
                int target;
                bool isArgs0Int = Int32.TryParse(args[0].ToString(), out target);
                if (isArgs0Int)
                {
                  MugShotEvent(source, target);
                }
            }
        }

            [EventHandler("mugShot")]
            void MugShotEvent(int source, int target)
            {
                Debug.WriteLine("Mugshot received by server.");
            if (target != 0) 
            {
                    SendClientModelNameForOutfit(source, target);
            }
            else
            {
                //Debug.WriteLine($"test 3");
                if (RoundHandling.targetThisGame != -1)
                {
                    SendClientModelNameForOutfit(source, RoundHandling.targetThisGame);
                }
                else SendClientModelNameForOutfit(source, source);

            }
        }

        void SendClientModelNameForOutfit(int source, int clientIdToSendModelOf)
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