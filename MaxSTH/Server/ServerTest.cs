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
                        if (args.Count == 1 && (args[0].ToString() == "check" || args[0].ToString() == "set" ))
                        {
                                if (args[0].ToString() == "check") TriggerClientEvent(Players[source], "CheckHealthStats");
                                else if (args[0].ToString() == "set") TriggerClientEvent(Players[source], "SetPlayerStats");
                        }
                        else if (args.Count == 2 && (args[0].ToString() == "check" || args[0].ToString() == "set" ))
                        {
                                if (args[0].ToString() == "check") TriggerClientEvent(Players[int.Parse(args[1].ToString())], "CheckHealthStats");
                                else if (args[0].ToString() == "set") TriggerClientEvent(Players[int.Parse(args[1].ToString())], "SetPlayerStats");
                        }
                        else TriggerClientEvent(Players[source], "ShowNotification", "do /test check/set (optional player ID)");
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
