using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace STHMaxzzzie.Server
{
        public class Test : BaseScript
        {
                // [Command("test", Restricted = false)]
                // void TestCommand(int source, List<object> args, string raw)
                // {
                        
                // }

                [Command("rpi", Restricted = true)]
                void rpi(int source, List<object> args, string raw)
                {
                        TriggerClientEvent(Players[source], "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"Did you mean to do /pri?" } });
                        foreach (Player notGil in Players)
                                if (notGil == Players[source])
                                {
                                        TriggerClientEvent(notGil, "chat:addMessage", new { color = new[] { 255, 0, 0 }, args = new[] { $"Hahaha, Gilly wrote /pri wrong! He typed /rpi." } });
                                }
                }


        }
}
