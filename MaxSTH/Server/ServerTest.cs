using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace STHMaxzzzie.Server
{
    public class Test : BaseScript
    {
        [Command("test", Restricted = false)]
        public void TestCommand(int source, List<object> args, string raw)
        {
          if (args.Count == 1 && args[0].ToString() == "bounce") TriggerClientEvent(Players[source], "gameBounce",0,true);
          if (args.Count == 1 && args[0].ToString() == "audio") TriggerClientEvent(Players[source], "clientTest1");
          else if (args.Count == 1) TriggerClientEvent(Players[source], "clientTest2", args[0].ToString());
          else TriggerClientEvent(Players[source], "chat:addMessage", new{color=new[]{255,153,153},args=new[]{$"Error test."}});
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
