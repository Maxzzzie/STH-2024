using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace STHMaxzzzie.Server
{
    public class Test : BaseScript
    {
        [Command("test", Restricted = true)]
        async void TestCommand(int source, List<object> args, string raw)
        {
                TriggerClientEvent(Players[source], "chat:addMessage", new{color=new[]{255,0,0},args=new[]{$"Maxzzzie is the best."}});
        }
    }
}
