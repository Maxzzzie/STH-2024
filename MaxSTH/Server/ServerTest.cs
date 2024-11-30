using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace STHMaxzzzie.Server
{
        public class Test : BaseScript
        {
                [Command("test", Restricted = true)]
                void TestCommand(int source, List<object> args, string raw)
                {
                        // if (args.Count == 1 && (args[0].ToString() == "check" || args[0].ToString() == "set" || args[0].ToString() == "hurt" || args[0].ToString() == "half" || args[0].ToString() == "full" ))
                        // {
                        //         if (args[0].ToString() == "check") TriggerClientEvent(Players[source], "CheckHealthStats");
                        //         else if (args[0].ToString() == "set") TriggerClientEvent(Players[source], "SetPlayerStats", true, true);
                        //         else if (args[0].ToString() == "hurt") TriggerClientEvent(Players[source], "HurtPlayer");
                        //         else if (args[0].ToString() == "half") TriggerClientEvent(Players[source], "HealHalf");
                        //         else if (args[0].ToString() == "full") TriggerClientEvent(Players[source], "HealCompletely");
                        // }
                        // else if (args.Count == 2 && (args[0].ToString() == "check" || args[0].ToString() == "set" || args[0].ToString() == "hurt" || args[0].ToString() == "half" || args[0].ToString() == "full" ))
                        // {
                        //         if (args[0].ToString() == "check") TriggerClientEvent(Players[int.Parse(args[1].ToString())], "CheckHealthStats");
                        //         else if (args[0].ToString() == "set") TriggerClientEvent(Players[int.Parse(args[1].ToString())], "SetPlayerStats", true, true);
                        //         else if (args[0].ToString() == "hurt") TriggerClientEvent(Players[int.Parse(args[1].ToString())], "HurtPlayer");
                        //         else if (args[0].ToString() == "half") TriggerClientEvent(Players[int.Parse(args[1].ToString())], "HealHalf");
                        //         else if (args[0].ToString() == "full") TriggerClientEvent(Players[int.Parse(args[1].ToString())], "HealCompletely");
                                
                        // }
                        // else TriggerClientEvent(Players[source], "ShowNotification", "do /test check/set/hurt/half/full (optional player ID)");
                }


                [Command("rpi", Restricted = true)]
                void rpi(int source, List<object> args, string raw)
                {
                        TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"Did you mean to do /pri?" } });
                        foreach (Player notGil in Players)
                                if (notGil != Players[source])
                                {
                                        TriggerClientEvent(notGil, "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"Hahaha, Gilly wrote /pri wrong! He typed /rpi." } });
                                }
                }


        }
}
