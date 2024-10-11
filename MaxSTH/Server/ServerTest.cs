using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace STHMaxzzzie.Server
{
    public class Test : BaseScript
    {
        [Command("test", Restricted = false)]
        void TestCommand(int source, List<object> args, string raw)
        {
            Debug.WriteLine($"This is the test command.");
 //TriggerClientEvent(player, "ShowNotification", "Test notification from server!", colors, true);
        }
    }
}
