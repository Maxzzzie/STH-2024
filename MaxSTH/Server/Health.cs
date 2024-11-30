using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace STHMaxzzzie.Server
{
    public class Health : BaseScript
    {

        [Command("togglehealth", Restricted = true)]
        void ToggleHealthCommand(int source, List<object> args, string raw)
        {
            int targetPlayerId;
            //targets client who called it
            if (args.Count == 1 && (args[0].ToString() == "check" || args[0].ToString() == "set" || args[0].ToString() == "hurt" || args[0].ToString() == "half" || args[0].ToString() == "full"))
            {
                if (args[0].ToString() == "check") TriggerClientEvent(Players[source], "CheckHealthStats");
                else if (args[0].ToString() == "set") TriggerClientEvent(Players[source], "SetPlayerStats", 300, 100);
                else if (args[0].ToString() == "hurt") TriggerClientEvent(Players[source], "HurtPlayer");
                else if (args[0].ToString() == "half") TriggerClientEvent(Players[source], "HealHalf");
                else if (args[0].ToString() == "full") TriggerClientEvent(Players[source], "HealCompletely");
            }
            else if (args.Count == 2 && (args[0].ToString() == "check" || args[0].ToString() == "set" || args[0].ToString() == "hurt" || args[0].ToString() == "half" || args[0].ToString() == "full") && Int32.TryParse(args[0].ToString(), out targetPlayerId))
            {
                //targets client who called it
                if (targetPlayerId == 0)
                {
                    if (args[0].ToString() == "check") TriggerClientEvent(Players[source], "CheckHealthStats");
                    else if (args[0].ToString() == "set") TriggerClientEvent(Players[source], "SetPlayerStats", 300, 100);
                    else if (args[0].ToString() == "hurt") TriggerClientEvent(Players[source], "HurtPlayer");
                    else if (args[0].ToString() == "half") TriggerClientEvent(Players[source], "HealHalf");
                    else if (args[0].ToString() == "full") TriggerClientEvent(Players[source], "HealCompletely");
                }
                
                //targets client who is the target
                else
                {
                    if (args[0].ToString() == "check") TriggerClientEvent(Players[targetPlayerId], "CheckHealthStats");
                    else if (args[0].ToString() == "set") TriggerClientEvent(Players[targetPlayerId], "SetPlayerStats", 300, 100);
                    else if (args[0].ToString() == "hurt") TriggerClientEvent(Players[targetPlayerId], "HurtPlayer");
                    else if (args[0].ToString() == "half") TriggerClientEvent(Players[targetPlayerId], "HealHalf");
                    else if (args[0].ToString() == "full") TriggerClientEvent(Players[targetPlayerId], "HealCompletely");
                }
            }

            //heals all players
            else if (args.Count == 2 && (args[0].ToString() == "check" || args[0].ToString() == "set" || args[0].ToString() == "hurt" || args[0].ToString() == "half" || args[0].ToString() == "full") && args[1].ToString() == "all")
            {
                if (args[0].ToString() == "check") TriggerClientEvent("CheckHealthStats");
                else if (args[0].ToString() == "set") TriggerClientEvent("SetPlayerStats", 300, 100);
                else if (args[0].ToString() == "hurt") TriggerClientEvent("HurtPlayer");
                else if (args[0].ToString() == "half") TriggerClientEvent("HealHalf");
                else if (args[0].ToString() == "full") TriggerClientEvent("HealCompletely");
            }

            else TriggerClientEvent(Players[source], "ShowNotification", "do /togglehealth check/set/hurt/half/full (optional player ID or \"all\").");
        }
    }
}